using Algorand.V2;
using Algorand.V2.Algod;
using Algorand.V2.Algod.Model;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
using System.Security.Cryptography;
using TokenIssue;

class Program
{
    private static IConfiguration Configuration { get; set; }

    static async Task<int> Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Configuration = builder.Build();

        // options
        var privateKeyOption = new Option<string?>(
            name: "--privatekey",
            description: "The private key in base64.");

        var unsignedTransactionOption = new Option<string?>(
            name: "--unsignedtx",
            description: "The unsigned transaction in base64 with MsgPack.");

        var creatorOption = new Option<string?>(
            name: "--creator",
            description: "The address to use as creator for the asset.");

        var managerOption = new Option<string?>(
            name: "--manager",
            description: "The address to use as manager for the asset.");

        var reserveOption = new Option<string?>(
            name: "--reserve",
            description: "The address to use as the clawback, freezer and reserve for the asset.");

        var defaultFrozenOption = new Option<bool>(
            name: "--defaultfrozen",
            description: "The default frozen state of the asset.");

        var signedTransactionOption = new Option<string?>(
            name: "--signedtx",
            description: "The signed transaction in base64 with MsgPack.");

        var roundOption = new Option<ulong>(
            name: "--round",
            description: "The round to take into account for validity time.");

        var validRoundsOption = new Option<ulong>(
            name: "--validrounds",
            description: "The amount of rounds to allow the transaction be be valid.",
            getDefaultValue: () => 1000);

        var fromAddressOption = new Option<string?>(
            name: "--from",
            description: "The address to use as the sender or initiator.");

        var destinationAddressOption = new Option<string?>(
            name: "--to",
            description: "The address to send to or manage.");

        var assetOption = new Option<ulong>(
            name: "--asset",
            description: "The asset id.");

        var optionalAssetOption = new Option<ulong?>(
            name: "--asset",
            description: "The asset id.");

        var amountOption = new Option<ulong?>(
            name: "--amount",
            description: "The amount in smallest unit (so * 100 for 2 decimals).");

        var noteOption = new Option<string?>(
            name: "--note",
            description: "The note to attach to the transaction.");

        var decimalOption = new Option<int>(
            name: "--decimals",
            description: "Amount of decimals.");

        var unitOption = new Option<string>(
            name: "--unit",
            description: "Unit name (e.g. EUR).");

        var nameOption = new Option<string>(
            name: "--name",
            description: "Asset name.");

        var overallLimitOption = new Option<long>(
            name: "--overalllimit",
            description: "Overall limit of the asset.");

        var urlOption = new Option<string>(
            name: "--url",
            description: "URL for the asset.",
            getDefaultValue: () => null);

        // commands
        var generateAccountCommand = new Command("generateaccount", "Generate a new account and expose the private key");
        generateAccountCommand.SetHandler(GenerateAccount);

        var createAssetCommand = new Command("createasset", "Create a new asset and sign it.")
        {
            defaultFrozenOption,
            creatorOption,
            managerOption,
            reserveOption,
            roundOption,
            validRoundsOption,
            noteOption,
            decimalOption,
            overallLimitOption,
            unitOption,
            nameOption,
            urlOption,
        };
        createAssetCommand.SetHandler(ctx =>
        {
            var defaultFrozen = ctx.ParseResult.GetValueForOption(defaultFrozenOption);
            var creator = ctx.ParseResult.GetValueForOption(creatorOption);
            var manager = ctx.ParseResult.GetValueForOption(managerOption);
            var reserve = ctx.ParseResult.GetValueForOption(reserveOption);
            var round = ctx.ParseResult.GetValueForOption(roundOption);
            var validRounds = ctx.ParseResult.GetValueForOption(validRoundsOption);
            var note = ctx.ParseResult.GetValueForOption(noteOption);
            var decimals = ctx.ParseResult.GetValueForOption(decimalOption);
            var overallLimit = ctx.ParseResult.GetValueForOption(overallLimitOption);
            var unit = ctx.ParseResult.GetValueForOption(unitOption);
            var name = ctx.ParseResult.GetValueForOption(nameOption);
            var url = ctx.ParseResult.GetValueForOption(urlOption);
            return CreateAssetTxAsync(creator, manager, reserve, round, validRounds, note, decimals, overallLimit, unit, name, url, defaultFrozen);
        });

        var optinCommand = new Command("optin", "Create a new opt-in transaction.")
        {
            fromAddressOption,
            assetOption,
            roundOption,
            validRoundsOption,
            noteOption
        };
        optinCommand.SetHandler(OptIn, fromAddressOption, assetOption, roundOption, validRoundsOption, noteOption);

        var unfreezeCommand = new Command("unfreeze", "Create a new unfreeze transaction.")
        {
            fromAddressOption,
            destinationAddressOption,
            assetOption,
            roundOption,
            validRoundsOption,
            noteOption
        };
        unfreezeCommand.SetHandler(
            (from, to, asset, round, valid, note) =>
                Unfreeze(from, to, asset, false, round, valid, note),
            fromAddressOption, destinationAddressOption, assetOption, roundOption, validRoundsOption, noteOption);

        var freezeCommand = new Command("freeze", "Create a new freeze transaction.")
        {
            fromAddressOption,
            destinationAddressOption,
            assetOption,
            roundOption,
            validRoundsOption,
            noteOption
        };
        freezeCommand.SetHandler(
            (from, to, asset, round, valid, note) =>
                Unfreeze(from, to, asset, true, round, valid, note),
            fromAddressOption, destinationAddressOption, assetOption, roundOption, validRoundsOption, noteOption);

        var manageAssetCommand = new Command("manageasset", "Create a new manage asset transaction.")
        {
            fromAddressOption,
            managerOption,
            reserveOption,
            creatorOption,
            assetOption,
            roundOption,
            validRoundsOption,
            noteOption,
            nameOption,
            unitOption,
            decimalOption,
            overallLimitOption
        };
        manageAssetCommand.SetHandler(ctx =>
        {
            //var defaultFrozen = ctx.ParseResult.GetValueForOption(defaultFrozenOption);
            var from = ctx.ParseResult.GetValueForOption(fromAddressOption);
            var creator = ctx.ParseResult.GetValueForOption(creatorOption);
            var manager = ctx.ParseResult.GetValueForOption(managerOption);
            var reserve = ctx.ParseResult.GetValueForOption(reserveOption);
            var round = ctx.ParseResult.GetValueForOption(roundOption);
            var validRounds = ctx.ParseResult.GetValueForOption(validRoundsOption);
            var note = ctx.ParseResult.GetValueForOption(noteOption);
            var decimals = ctx.ParseResult.GetValueForOption(decimalOption);
            var overallLimit = ctx.ParseResult.GetValueForOption(overallLimitOption);
            var unit = ctx.ParseResult.GetValueForOption(unitOption);
            var name = ctx.ParseResult.GetValueForOption(nameOption);
            //var url = ctx.ParseResult.GetValueForOption(urlOption);
            var asset = ctx.ParseResult.GetValueForOption(assetOption);
            return ManageAsset(from, manager, reserve, creator, asset, round, validRounds, note, name, unit, decimals, overallLimit);
        });


        var paymentCommand = new Command("payment", "Create a new payment transaction.")
        {
            fromAddressOption,
            destinationAddressOption,
            optionalAssetOption,
            amountOption,
            roundOption,
            validRoundsOption,
            noteOption
        };
        paymentCommand.SetHandler(Payment, fromAddressOption, destinationAddressOption, optionalAssetOption, amountOption, roundOption, validRoundsOption, noteOption);

        var clawbackCommand = new Command("clawback", "Create a new clawback transaction.")
        {
            fromAddressOption,
            destinationAddressOption,
            assetOption,
            amountOption,
            roundOption,
            validRoundsOption,
            noteOption
        };
        clawbackCommand.SetHandler(Clawback, fromAddressOption, destinationAddressOption, assetOption, amountOption, roundOption, validRoundsOption, noteOption);

        var signCommand = new Command("sign", "Sign a msgpack base64 encoded transaction.")
        {
            privateKeyOption,
            unsignedTransactionOption
        };
        signCommand.SetHandler(SignTransaction, privateKeyOption, unsignedTransactionOption);

        var submitCommand = new Command("submit", "Submit a signed base64 msgpack encoded transaction")
        {
            signedTransactionOption
        };
        submitCommand.SetHandler(SubmitTransactionAsync, signedTransactionOption);

        // root
        var rootCommand = new RootCommand("Quantoz Pay Issuing App");
        rootCommand.AddCommand(generateAccountCommand);
        rootCommand.AddCommand(signCommand);
        rootCommand.AddCommand(createAssetCommand);
        rootCommand.AddCommand(optinCommand);
        rootCommand.AddCommand(freezeCommand);
        rootCommand.AddCommand(unfreezeCommand);
        rootCommand.AddCommand(manageAssetCommand);
        rootCommand.AddCommand(paymentCommand);
        rootCommand.AddCommand(clawbackCommand);
        rootCommand.AddCommand(submitCommand);

        return await rootCommand.InvokeAsync(args);
    }

    private static DefaultApi GetAlgorandNode()
    {
        // read out settings
        var settings = new Settings();
        Configuration.GetRequiredSection("algod").Bind(settings);

        var httpClient = HttpClientConfigurator.ConfigureHttpClient(settings.ApiUrl, settings.ApiKey, tokenHeader: settings.ApiKeyHeader);
        var algod = new DefaultApi(httpClient) { BaseUrl = settings.ApiUrl };

        return algod;
    }

    private static TransactionParametersResponse GetManualParams(ulong round)
    {
        return new TransactionParametersResponse
        {
            ConsensusVersion = "https://github.com/algorandfoundation/specs/tree/abd3d4823c6f77349fc04c3af7b1e99fe4df699f",
            MinFee = 1000,
            Fee = 0,
            GenesisId = "mainnet-v1.0",
            GenesisHash = Convert.FromBase64String("wGHE2Pwdvd7S12BL5FaOP20EGYesN73ktiC1qzkkit8="),
            LastRound = round
        };
    }

    static async Task CreateAssetTxAsync(string? creator, string? manager, string? reserve, ulong round, ulong roundValidity, string? note, int decimals, long overallLimit, string? unit, string? name, string? url, bool? defaultFrozen)
    {
        if (creator == null)
        {
            Console.WriteLine("Failed to load creator.");
            return;
        }

        if (round == 0)
        {
            await Console.Out.WriteLineAsync("Please supply latest round.");
            return;
        }

        if (string.IsNullOrEmpty(manager))
        {
            await Console.Out.WriteLineAsync("Please supply manager.");
            return;
        }

        if (string.IsNullOrEmpty(reserve))
        {
            await Console.Out.WriteLineAsync("Please supply reserve address.");
            return;
        }

        if (decimals == 0)
        {
            await Console.Out.WriteLineAsync("Zero (0) decimals provided.");
        }

        if (overallLimit == 0)
        {
            await Console.Out.WriteLineAsync("Please supply overall limit.");
            return;
        }

        if (string.IsNullOrEmpty(unit))
        {
            await Console.Out.WriteLineAsync("Please supply unit.");
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            await Console.Out.WriteLineAsync("Please supply name.");
            return;
        }

        if (defaultFrozen is null)
        {
            await Console.Out.WriteLineAsync("Please supply default frozen.");
            return;
        }

        // we need to convert to microEuro
        var overallLimitMicroEur = overallLimit * (decimal)Math.Pow(10, decimals);

        var assetParams = new AssetParams
        {
            Name = name,
            UnitName = unit,
            DefaultFrozen = defaultFrozen,

            Total = (ulong)overallLimitMicroEur,
            Decimals = decimals,
            MetadataHash = GenerateRandomBytes(32),

            Url = url,

            Creator = creator,
            Clawback = reserve,
            Freeze = reserve,
            Manager = manager,
            Reserve = reserve
        };

        var transactionParams = GetManualParams(round);

        var assetTxn = Algorand.Utils.GetCreateAssetTransaction(assetParams, transactionParams, message: note ?? "");

        // set validity time to 1000 blocks from now
        assetTxn.firstValid = transactionParams.LastRound;
        assetTxn.lastValid = transactionParams.LastRound + roundValidity;
        assetTxn.assetParams.metadataHash = null;

        var encodedSigned = Algorand.Encoder.EncodeToMsgPack(assetTxn);

        Console.WriteLine("Unsigned tx on next line:");
        Console.WriteLine(Convert.ToBase64String(encodedSigned));
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        var bytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return bytes;
    }

    static async Task OptIn(string? address, ulong asset, ulong round, ulong roundValidity, string? note)
    {
        if (address == null)
        {
            Console.WriteLine("Please supply address.");
            return;
        }

        if (asset == 0)
        {
            await Console.Out.WriteLineAsync("Please supply asset.");
            return;
        }

        if (round == 0)
        {
            await Console.Out.WriteLineAsync("Please supply latest round.");
            return;
        }

        if (roundValidity == 0)
        {
            await Console.Out.WriteLineAsync("Please supply round validity.");
            return;
        }

        var transactionParams = GetManualParams(round);

        var assetTxn = Algorand.Utils.GetAssetOptingInTransaction(new Algorand.Address(address), asset, transactionParams, message: note ?? "");

        // set validity time to 1000 blocks from now
        assetTxn.firstValid = transactionParams.LastRound;
        assetTxn.lastValid = transactionParams.LastRound + roundValidity;

        var encodedSigned = Algorand.Encoder.EncodeToMsgPack(assetTxn);

        Console.WriteLine("Unsigned tx on next line:");
        Console.WriteLine(Convert.ToBase64String(encodedSigned));
    }

    static async Task ManageAsset(string? fromAddress, string? manager, string? reserve, string? creator, ulong asset, ulong round, ulong roundValidity, string? note, string? name, string? unit, int? decimals, long? overallLimit)
    {
        if (manager == null || reserve == null || fromAddress == null || creator == null)
        {
            Console.WriteLine("Please supply `manager`, `reserve`, `creator` and `from` address.");
            return;
        }

        if (asset == 0)
        {
            await Console.Out.WriteLineAsync("Please supply asset.");
            return;
        }

        if (round == 0)
        {
            await Console.Out.WriteLineAsync("Please supply latest round.");
            return;
        }

        if (roundValidity == 0)
        {
            await Console.Out.WriteLineAsync("Please supply round validity.");
            return;
        }

        if (decimals is null)
        {
            await Console.Out.WriteLineAsync("Please supply decimals.");
            return;
        }

        if (overallLimit is null)
        {
            await Console.Out.WriteLineAsync("Please supply overall limit.");
            return;
        }

        // we need to convert to microEuro
        var overallLimitMicroEur = overallLimit * (decimal)Math.Pow(10, decimals.Value);

        var assetParams = new AssetParams
        {
            Clawback = reserve,
            Creator = creator,
            Freeze = reserve,
            Reserve = reserve,
            Manager = manager,

            // needed for sdk
            Name = name,
            UnitName = unit,
            //DefaultFrozen = true,

            Total = (ulong)overallLimitMicroEur,
            Decimals = decimals.Value,
            //MetadataHash = null,

            //Url = url,
        };

        var configAsset = new Asset()
        {
            Index = asset,
            Params = assetParams
        };

        var transactionParams = GetManualParams(round);

        var assetTxn = Algorand.Utils.GetConfigAssetTransaction(new Algorand.Address(fromAddress), configAsset, transactionParams, message: note ?? "");

        // set validity time to 1000 blocks from now
        assetTxn.firstValid = transactionParams.LastRound;
        assetTxn.lastValid = transactionParams.LastRound + roundValidity;

        var encodedSigned = Algorand.Encoder.EncodeToMsgPack(assetTxn);

        Console.WriteLine("Unsigned tx on next line:");
        Console.WriteLine(Convert.ToBase64String(encodedSigned));
    }

    static async Task Payment(string? fromAddress, string? destinationAddress, ulong? asset, ulong? amount, ulong round, ulong roundValidity, string? note)
    {
        if (fromAddress == null || destinationAddress == null)
        {
            Console.WriteLine("Please supply `from` and `to` address.");
            return;
        }

        if (amount is null)
        {
            await Console.Out.WriteLineAsync("Please supply `amount`.");
            return;
        }

        if (round == 0)
        {
            await Console.Out.WriteLineAsync("Please supply latest round.");
            return;
        }

        if (roundValidity == 0)
        {
            await Console.Out.WriteLineAsync("Please supply round validity.");
            return;
        }

        var transactionParams = GetManualParams(round);

        var assetTxn = asset is null
            ? Algorand.Utils.GetPaymentTransaction(
                new Algorand.Address(fromAddress), new Algorand.Address(destinationAddress),
                amount.Value, note ?? "", transactionParams)
            : Algorand.Utils.GetTransferAssetTransaction(
                new Algorand.Address(fromAddress), new Algorand.Address(destinationAddress),
                asset, amount.Value, transactionParams, message: note ?? "");

        // set validity time to 1000 blocks from now
        assetTxn.firstValid = transactionParams.LastRound;
        assetTxn.lastValid = transactionParams.LastRound + roundValidity;

        var encodedSigned = Algorand.Encoder.EncodeToMsgPack(assetTxn);

        Console.WriteLine("Unsigned tx on next line:");
        Console.WriteLine(Convert.ToBase64String(encodedSigned));
    }


    static async Task Clawback(string? fromAddress, string? destinationAddress, ulong asset, ulong? amount, ulong round, ulong roundValidity, string? note)
    {
        if (fromAddress == null || destinationAddress == null)
        {
            Console.WriteLine("Please supply `from` and `to` address.");
            return;
        }

        if (asset == 0)
        {
            await Console.Out.WriteLineAsync("Please supply asset.");
            return;
        }

        if (amount is null)
        {
            await Console.Out.WriteLineAsync("Please supply `amount`.");
            return;
        }

        if (round == 0)
        {
            await Console.Out.WriteLineAsync("Please supply latest round.");
            return;
        }

        if (roundValidity == 0)
        {
            await Console.Out.WriteLineAsync("Please supply round validity.");
            return;
        }

        var transactionParams = GetManualParams(round);

        var assetTxn = Algorand.Utils.GetRevokeAssetTransaction(new Algorand.Address(destinationAddress), 
            new Algorand.Address(fromAddress), new Algorand.Address(destinationAddress), asset, amount.Value, 
            transactionParams, message: note ?? "");

        // set validity time to 1000 blocks from now
        assetTxn.firstValid = transactionParams.LastRound;
        assetTxn.lastValid = transactionParams.LastRound + roundValidity;

        var encodedSigned = Algorand.Encoder.EncodeToMsgPack(assetTxn);

        Console.WriteLine("Unsigned tx on next line:");
        Console.WriteLine(Convert.ToBase64String(encodedSigned));
    }


    static async Task Unfreeze(string? from, string? to, ulong asset, bool freeze, ulong round, ulong roundValidity, string? note)
    {
        if (from == null)
        {
            Console.WriteLine("Please supply `from` address.");
            return;
        }

        if (to == null)
        {
            Console.WriteLine("Please supply `to` address.");
            return;
        }

        if (asset == 0)
        {
            await Console.Out.WriteLineAsync("Please supply asset.");
            return;
        }

        if (round == 0)
        {
            await Console.Out.WriteLineAsync("Please supply latest round.");
            return;
        }

        if (roundValidity == 0)
        {
            await Console.Out.WriteLineAsync("Please supply round validity.");
            return;
        }

        var transactionParams = GetManualParams(round);

        var assetTxn = Algorand.Utils.GetFreezeAssetTransaction(new Algorand.Address(from), new Algorand.Address(to), asset, freeze, transactionParams, message: note ?? "");

        // set validity time to 1000 blocks from now
        assetTxn.firstValid = transactionParams.LastRound;
        assetTxn.lastValid = transactionParams.LastRound + roundValidity;

        var encodedSigned = Algorand.Encoder.EncodeToMsgPack(assetTxn);

        Console.WriteLine("Unsigned tx on next line:");
        Console.WriteLine(Convert.ToBase64String(encodedSigned));
    }

    static void GenerateAccount()
    {
        var account = new Algorand.Account();
        Console.WriteLine("Address on next line:");
        Console.WriteLine($"{account.Address}");

        Console.WriteLine($"PrivateKey on next line:");
        Console.WriteLine(Convert.ToBase64String(account.GetClearTextPrivateKey()));
    }

    static async Task SubmitTransactionAsync(string? signedTx)
    {
        if (signedTx == null)
        {
            Console.WriteLine("Failed to load signed tx.");
            return;
        }

        var tx = Algorand.Encoder.DecodeFromMsgPack<Algorand.SignedTransaction>(Convert.FromBase64String(signedTx));
        var algod = GetAlgorandNode();

        var txId = await Algorand.Utils.SubmitTransaction(instance: algod, signedTx: tx);

        Console.WriteLine($"Waiting for {txId.TxId}");

        await Algorand.Utils.WaitTransactionToComplete(algod, txId.TxId);

        await Console.Out.WriteLineAsync("Successfully submitted tx.");
    }

    private static Algorand.Account LoadAccount(string privKeyBase64)
    {
        var privKey = Convert.FromBase64String(privKeyBase64);
        var account = new Algorand.Account(privKey);
        return account;
    }

    static void SignTransaction(string? privKey, string? txEncoded)
    {
        if (privKey == null)
        {
            Console.WriteLine("Failed to load private key.");
            return;
        }

        if (txEncoded == null)
        {
            Console.WriteLine("Failed to load unsinged tx.");
            return;
        }

        var tx = Algorand.Encoder.DecodeFromMsgPack<Algorand.Transaction>(Convert.FromBase64String(txEncoded));

        var account = LoadAccount(privKey);

        // sign transaction
        Console.WriteLine($"Signing with {account.Address}");

        var signedTx = account.SignTransaction(tx);

        var encodedSigned = Algorand.Encoder.EncodeToMsgPack(signedTx);

        Console.WriteLine("Signed tx on next line:");
        Console.WriteLine(Convert.ToBase64String(encodedSigned));
    }
}

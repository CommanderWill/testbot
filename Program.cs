using Discord;
using Discord.Net;
using Discord.WebSocket;
using System.IO;
using Newtonsoft.Json;
using MyNameSpace;
using FileManagementCS;
using Banking;

Bot bot = new();
FileManagementCS.FILE_MANAGEMENT fm = new();


// Output Bot App Directory
string output = fm.applicationDirectory;
Console.WriteLine(output);
Console.WriteLine('\n');

Banking.Banking banking = new();

// Run Bot
await bot.Run();


// Bot Main Code
namespace MyNameSpace
{
    public class Bot
    {
        static readonly string TokenPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "token.txt");

        DiscordSocketClient _client;



        private string Token
        {
            get
            {
                using var sr = File.OpenText(TokenPath);
                var returnVar = sr.ReadToEnd().ReplaceLineEndings("");
                sr.Close();
                return returnVar;                
            }
        }

        public Bot()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
        }

        public async Task Run()
        {
            await _client.LoginAsync(TokenType.Bot, Token);
            await _client.StartAsync();
            await Task.Delay(-1);

        }
        public Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }


        // Commands
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                // Depricated
                case "first-command":
                    await command.RespondAsync($"Depricated");
                    break;
                case "transferfunds":
                    await command.RespondAsync($"Depricated");
                    break;
                case "transferfunds-role":
                    await command.RespondAsync($"Depricated");
                    break;

                // Current
                case "testing":
                    await HandleListRoleCommand(command);
                    break;                
                case "account":
                    await handleAccount(command);
                    break;                
                case "bank-admin-user":
                    await handleFundsTransfer(command);
                    break;
                case "bank-admin-role":
                    await handleFundsTransfer_Role(command);
                    break;
            }
        }

        private async Task handleFundsTransfer(SocketSlashCommand command)
        {
            Banking.Banking banking = new();
            //set transaction variables
            string fromAccount = Convert.ToString(command.Data.Options.ElementAt(0).Value);
            string toAccount = Convert.ToString(command.Data.Options.ElementAt(1).Value);
            int transferAmount = Convert.ToInt32(command.Data.Options.ElementAt(2).Value);
            string sTransferAmount = Convert.ToString(transferAmount);
            string transferDetails = Convert.ToString(command.Data.Options.ElementAt(3).Value);
            string guildID = Convert.ToString(command.GuildId);

            //Run Funds Transfer
            banking.transferFunds(fromAccount, toAccount, transferAmount, transferDetails, guildID);
            string transferMessage =
                    "**------------------------------------------------------------------**" + System.Environment.NewLine +
                    "**Receipt**:" + System.Environment.NewLine +
                    "**------------------------------------------------------------------**" + System.Environment.NewLine +
                    "**From:** " + fromAccount + System.Environment.NewLine +
                    "**To:** " + toAccount + System.Environment.NewLine +
                    "**Amount:** $" + sTransferAmount + System.Environment.NewLine +
                    "**Details** " + transferDetails + System.Environment.NewLine +
                     "**------------------------------------------------------------------**" + System.Environment.NewLine +
                     fromAccount + "**New Balance $** " + banking.getBalance(fromAccount, guildID) + System.Environment.NewLine +
                     toAccount + "**New Balance $** " + banking.getBalance(toAccount, guildID) + System.Environment.NewLine +
                    "**------------------------------------------------------------------**" + System.Environment.NewLine;
            Console.WriteLine(transferMessage);
            await command.RespondAsync(transferMessage);
        }

        private async Task handleAccount(SocketSlashCommand command)
        {
            Banking.Banking banking = new();
            //Determine Command Set
            var commandSet = command.Data.Options.First().Name;
            if (commandSet == "transaction")
            { // Transaction Set
                //Set Transaction Set Variables
                string transactionAccountName = Convert.ToString(command.Data.Options.First().Options.ElementAt(0).Value);
                int transactionType = Convert.ToInt32(command.Data.Options.First().Options.ElementAt(1).Value);
                int transactionAmount = Convert.ToInt32(command.Data.Options.First().Options.ElementAt(2).Value);
                string transactionDetails = Convert.ToString(command.Data.Options.First().Options.ElementAt(3).Value);
                string guildID = Convert.ToString(command.GuildId);
                string transactionTransactor = Convert.ToString(command.User);
                string sTransactionType = "";
                string sTransactionAmount = Convert.ToString(transactionAmount);                
                switch (transactionType)
                {
                    case 1:
                        sTransactionType = "deposit";
                        break;
                    case 2:
                        sTransactionType = "spend";
                        break;
                }              
                

                if (!banking.checkAccountExistence(transactionAccountName, guildID))
                {
                    banking.createAccount(transactionAccountName, guildID);
                }
                banking.accountTransaction(transactionAccountName, sTransactionType, transactionAmount, transactionDetails, transactionTransactor, guildID, true);
                string testResponse =
                    "**------------------------------------------------------------------**" + System.Environment.NewLine +
                    "**Receipt**:" + System.Environment.NewLine +
                    "**------------------------------------------------------------------**" + System.Environment.NewLine +
                    "**Account:** " + transactionAccountName + System.Environment.NewLine +
                    "**Type:** " + sTransactionType + System.Environment.NewLine +
                    "**Amount:** $" + sTransactionAmount + System.Environment.NewLine +
                    "**Details** " + transactionDetails + System.Environment.NewLine +
                     "**------------------------------------------------------------------**" + System.Environment.NewLine +
                      "**New Balance** " + banking.getBalance(transactionAccountName, guildID) + System.Environment.NewLine +
                    "**------------------------------------------------------------------**" + System.Environment.NewLine;
                Console.WriteLine(testResponse);
                await command.RespondAsync(testResponse);
            } else
            { // Add in guildID stuff
                if(commandSet == "check")
                {
                    var commandSpecific = command.Data.Options.First().Options.First().Name;
                    Console.WriteLine(commandSpecific);
                    var secondCommandSet = command.Data.Options.First().Options.First();
                    var secondCommandSetName = command.Data.Options.First().Options.First().Name;
                    string guildID = Convert.ToString(command.GuildId);
                    Console.WriteLine(secondCommandSetName);
                    if (commandSpecific == "balance")
                    {
                        await command.RespondAsync("**Account: **" + Convert.ToString(secondCommandSet.Options.First().Value) + "\n**Balance:** $" + banking.getBalance(Convert.ToString(command.Data.Options.First().Options.First().Options.First().Value), guildID) );
                    } else if (commandSpecific == "transaction-logs")
                    {
                        Console.WriteLine(secondCommandSet.Options.First().Value);
                        await command.RespondAsync(banking.getRecentTransactions(Convert.ToString(secondCommandSet.Options.First().Value), guildID));
                    }
                }
            }  
        }

        private async Task HandleListRoleCommand(SocketSlashCommand command)
        {
            // We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            // We remove the everyone role and select the mention of each role.
            var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Roles")
                .WithDescription(roleList)
                .WithColor(Color.DarkRed)
                .WithCurrentTimestamp();

            // Now, Let's respond with the embed.
            await command.RespondAsync(embed: embedBuiler.Build());

            // Log this action in the console
            string msg = "User: " + guildUser.ToString() + System.Environment.NewLine + "Roles: " + roleList;
            Console.WriteLine(msg.ToString());
        }

        private async Task handleFundsTransfer_Role(SocketSlashCommand command)
        {
            //Setup Banking Class for Banking Operations
            Banking.Banking banking = new();

            ///----------GET ALL VARIABLES READY------------
            // Get Guild ID
            string guildID = Convert.ToString(command.GuildId);
            //Get the account the funds are being transfered from
            string fromAccount = Convert.ToString(command.Data.Options.ElementAt(0).Value);

            //Get and Convert List of Members Who Have The Role Then Prepare Other Member Variables
            SocketRole selectedRole = (SocketRole)command.Data.Options.ElementAt(1).Value; 
            IEnumerable<SocketGuildUser> roleMembers = selectedRole.Members;
            SocketGuildUser[] roleMembersArray = roleMembers.ToArray();
            string roleMember = "";
            string[] sRoleMembersArray = new string[roleMembersArray.Length];
            string roleMembersList = "";

            //Get Transfer Amount
            int transferAmount = Convert.ToInt32(command.Data.Options.ElementAt(2).Value);
            string sTransferAmount = Convert.ToString(transferAmount);

            //Get Distribution Type
            int distributionType = Convert.ToInt32(command.Data.Options.ElementAt(3).Value);

            //Get the Transfer Details
            string transferDetails = Convert.ToString(command.Data.Options.ElementAt(4).Value);

            if(distributionType == 1)
            { //even split distribution
                int splitTransferAmount = (transferAmount /(roleMembersArray.Length));
                for (int i = 0; i < roleMembersArray.Length; i++)
                {
                    roleMember = roleMembersArray[i].ToString();
                    banking.transferFunds(fromAccount, roleMember, splitTransferAmount, transferDetails, guildID);
                    sRoleMembersArray[i] = roleMember;
                    roleMembersList = roleMembersList + ", " + roleMember;
                };
            }
            else
            { //per-each distribution
                for (int i = 0; i < roleMembersArray.Length; i++)
                {
                    roleMember = roleMembersArray[i].ToString();
                    banking.transferFunds(fromAccount, roleMember, transferAmount, transferDetails, guildID);
                    sRoleMembersArray[i] = roleMember;
                    roleMembersList = roleMembersList + ", " + roleMember;
                };
            }
            string transferMessage =
                   "**------------------------------------------------------------------**" + System.Environment.NewLine +
                   "**Receipt**:" + System.Environment.NewLine +
                   "**------------------------------------------------------------------**" + System.Environment.NewLine +
                   "**From:** " + fromAccount + System.Environment.NewLine +
                   "**To:** " + roleMembersList + System.Environment.NewLine +
                   "**Amount:** $" + sTransferAmount + System.Environment.NewLine +
                   "**Details** " + transferDetails + System.Environment.NewLine +
                    "**------------------------------------------------------------------**" + System.Environment.NewLine;
            Console.WriteLine(transferMessage);
            await command.RespondAsync(transferMessage);
        }
        private async Task Client_Ready()
        {
            FileManagementCS.FILE_MANAGEMENT fm = new();

            //Test command, was used to learn some stuff about commands but is not meant to be a primary function of the bot
            var testing = new SlashCommandBuilder()
                .WithName("testing") // Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
                .WithDescription("Yaet") // Descriptions can have a max length of 100.
                .AddOption("user", ApplicationCommandOptionType.User, "Lists User Roles", isRequired: true);

            //Transfer funds from one account to another account
            var accountFundsTransfer = new SlashCommandBuilder()
                .WithName("bank-admin-user")
                .WithDescription("transfer funds between two accounts")
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .AddOption("fromaccount", ApplicationCommandOptionType.String, "Account the money is from", isRequired: true)
                .AddOption("toaccount", ApplicationCommandOptionType.String, "Account the money is going to", isRequired: true)
                .AddOption("amount", ApplicationCommandOptionType.Integer, "Amount to Transfer", isRequired: true)
                .AddOption("details", ApplicationCommandOptionType.String, "Details of Funds Transfer", isRequired: true);

            //Transfer funds from one account to everyone with a role
            var roleFundsTransfer = new SlashCommandBuilder()
                .WithName("bank-admin-role")
                .WithDescription("transfer funds from one account to all accounts with the specified role")
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .AddOption("fromaccount", ApplicationCommandOptionType.String, "Account the money is from", isRequired: true)
                .AddOption("torole", ApplicationCommandOptionType.Role, "Role the money is going to", isRequired: true)
                .AddOption("amount", ApplicationCommandOptionType.Integer, "Amount to Transfer", isRequired: true)
                .AddOption(new SlashCommandOptionBuilder()
                                .WithName("distribution_type")
                                .WithDescription("What method to distribute?")
                                .WithRequired(true)
                                .AddChoice("even_split", 1)
                                .AddChoice("per_each", 2)
                                .WithType(ApplicationCommandOptionType.Integer)
                            )
                .AddOption("details", ApplicationCommandOptionType.String, "Details of Funds Transfer", isRequired: true);

            //Basic Account Transatction (Adds/Removes w/o transfer)
            var accountTransaction = new SlashCommandBuilder()
                .WithName("account")
                .WithDescription("Perform Account Operations")
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .AddOption(new SlashCommandOptionBuilder()
                        .WithName("transaction")
                        .WithDescription("Perform an Account Transaction")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                            .AddOption("account_name", ApplicationCommandOptionType.String, "Which Account to Check", isRequired: true)
                            .AddOption(new SlashCommandOptionBuilder()
                                .WithName("type")
                                .WithDescription("What Transaction Type")
                                .WithRequired(true)
                                .AddChoice("Deposit", 1)
                                .AddChoice("Spend", 2)
                                .WithType(ApplicationCommandOptionType.Integer)
                            )
                            .AddOption("amount", ApplicationCommandOptionType.Integer, "Amount to Deposit", isRequired: true)
                            .AddOption("details", ApplicationCommandOptionType.String, "Details of Transaction", isRequired: true)

                )
                .AddOption(new SlashCommandOptionBuilder()
                        .WithName("check")
                        .WithDescription("Check Account Balance")
                        .WithType(ApplicationCommandOptionType.SubCommandGroup)
                            .AddOption(new SlashCommandOptionBuilder()
                                .WithName("balance")
                                .WithDescription("Deposit Money")
                                .WithType(ApplicationCommandOptionType.SubCommand)
                                .AddOption("account_name", ApplicationCommandOptionType.String, "Which Account to Check", isRequired: true)
                                )
                            .AddOption(new SlashCommandOptionBuilder()
                                .WithName("transaction-logs")
                                .WithDescription("Look at all past transactions")
                                .WithType(ApplicationCommandOptionType.SubCommand)
                                .AddOption("account_name", ApplicationCommandOptionType.String, "Which Account to Check Get Transaction Logs", isRequired: true)
                            )
                );


            //Init Commands within Servers
            ulong guildID;
            int guildNum = 0;
            int guildNumTot = fm.fileLinesCheck(fm.applicationDirectory, "server_id.txt");
            do
            {
                // Set Guild ID
                guildID = Convert.ToUInt64(fm.fileReadSpecific(fm.applicationDirectory, "server_id.txt", guildNum, guildNum));

                
                try
                {
                    // Generate Commands
                    await _client.Rest.CreateGuildCommand(testing.Build(), guildID);
                    await _client.Rest.CreateGuildCommand(accountFundsTransfer.Build(), guildID);
                    await _client.Rest.CreateGuildCommand(roleFundsTransfer.Build(), guildID);
                    await _client.Rest.CreateGuildCommand(accountTransaction.Build(), guildID);
                }
                catch (HttpException exception)
                {
                    var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                    Console.WriteLine(json);
                }
                guildNum += 2;
            } while (guildNum <= guildNumTot);
        }
    }
}

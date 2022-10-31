﻿using Discord;
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
                case "first-command":
                    await command.RespondAsync($"Depricated");
                    break;
                case "testing":
                    await HandleListRoleCommand(command);
                    await COR_Roles(command);
                    break;
                case "account":
                    await handleAccount(command);
                    break;
            }
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
                
                

                if (!banking.checkAccountExistence(transactionAccountName))
                {
                    banking.createAccount(transactionAccountName);
                }
                banking.accountTransaction(transactionAccountName, sTransactionType, transactionAmount, transactionDetails, transactionTransactor);
                string testResponse =
                    "**------------------------------------------------------------------**" + System.Environment.NewLine +
                    "**Receipt**:" + System.Environment.NewLine +
                    "**------------------------------------------------------------------**" + System.Environment.NewLine +
                    "**Account:** " + transactionAccountName + System.Environment.NewLine +
                    "**Type:** " + sTransactionType + System.Environment.NewLine +
                    "**Amount:** $" + sTransactionAmount + System.Environment.NewLine +
                    "**Details** " + transactionDetails + System.Environment.NewLine +
                     "**------------------------------------------------------------------**" + System.Environment.NewLine +
                      "**New Balance** " + banking.getBalance(transactionAccountName) + System.Environment.NewLine +
                    "**------------------------------------------------------------------**" + System.Environment.NewLine;
                Console.WriteLine(testResponse);
                await command.RespondAsync(testResponse);
            } else
            { // Get Set
                if(commandSet == "check")
                {
                    var commandSpecific = command.Data.Options.First().Options.First().Name;
                    Console.WriteLine(commandSpecific);
                    var secondCommandSet = command.Data.Options.First().Options.First();
                    var secondCommandSetName = command.Data.Options.First().Options.First().Name;
                    Console.WriteLine(secondCommandSetName);
                    if (commandSpecific == "balance")
                    {
                        await command.RespondAsync("**Account: **" + Convert.ToString(secondCommandSet.Options.First().Value) + "\n**Balance:** $" + banking.getBalance(Convert.ToString(command.Data.Options.First().Options.First().Options.First().Value)) );
                    } else if (commandSpecific == "transaction-logs")
                    {
                        Console.WriteLine(secondCommandSet.Options.First().Value);
                        await command.RespondAsync(banking.getRecentTransactions(Convert.ToString(secondCommandSet.Options.First().Value)));
                    }
                }
            }  
        }

        private async Task COR_Roles(SocketSlashCommand command) //Console Output Roles
        {
            // We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            // We remove the everyone role and select the mention of each role.
            var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            string msg = "User: " + guildUser.ToString() + System.Environment.NewLine + "Roles: " + roleList;
            Console.WriteLine(msg.ToString());
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
        }

        private async Task Client_Ready()
        {
            FileManagementCS.FILE_MANAGEMENT fm = new();
            ulong guildID = 870858496160174080;
            //Testing guild ID: 870858496160174080
            //TPRP ID: 778444685952680027
            //AMD ID: 1011055042591207465
            var testing = new SlashCommandBuilder()// Next, lets create our slash command builder. This is like the embed builder but for slash commands.
                .WithName("testing") // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
                .WithDescription("Yaet") // Descriptions can have a max length of 100.
                .AddOption("user", ApplicationCommandOptionType.User, "Lists User Roles", isRequired: true);

            // Detail Command
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
                           .AddOption("details", ApplicationCommandOptionType.String, "Details of Income", isRequired: true)
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

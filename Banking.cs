using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManagementCS;



namespace Banking
{
    internal class Banking:FILE_MANAGEMENT
    {
        DateTime dateN;

        protected string templateDir = subClassAppDirectory + "\\templates";
        string accountDir = subClassAppDirectory + "\\accounts"; //Outdated
        string guildsDir = subClassAppDirectory + "\\guilds";
        string accountTemplate = "account.txt";

        //=============FILE STRUCTURE============
        //
        //Templates
        //Guilds
        //     ┗GuildID
        //              ┣Accounts
        //              ┗Logs
        //    
        //
        //======================================


        //Kept Old Functions
        public string dateNumber()
        {
            dateN = DateTime.Now;
            string dateNumber = dateN.ToString("yyyy") + dateN.ToString("MM") + dateN.ToString("dd") + dateN.ToString("HH") + dateN.ToString("mm") + dateN.ToString("ss");
            return dateNumber;
        }

        //Version 1.0 Functions (Outdated)
        public void createAccount(string accountName)
        {
            string accountFile = accountName + ".txt";

            fileCopy(templateDir, accountTemplate, accountDir, accountFile, false);
        }
        public void accountTransaction(string accountName, string type, int amount, string details, string transactor) 
        {
            string accountDir = subClassAppDirectory + "\\accounts";
            string accountTransactionLogsFolder = subClassAppDirectory + "\\transaction_logs";
            string accountTransactionLog = accountTransactionLogsFolder + "\\" + accountName + ".txt";
            Console.WriteLine(accountTransactionLog);

            string accountFile = accountName + ".txt";
            if (checkAccountExistence(accountName) == false)
            {
                createAccount(accountName);
            }
            string logMessage = "[" + dateNumber() + "] {Account: " + accountName + "; Transactor: " + transactor + "; Type: " + type + "; Amount: $" + amount + "; Details: " + details + ";}";

            int balance = Convert.ToInt32(getBalance(accountName));
            if (type == "spend")
            {
                amount = amount * -1;
            }
            int newBalance = balance + amount;
            fileEditExistingLine(accountDir, accountFile, 2, Convert.ToString(newBalance));

            if (fileExistCheck(accountTransactionLog))
            {
                fileAddLine(accountTransactionLogsFolder, accountFile, logMessage);
            }
            else
            {
                fileCreate(accountTransactionLogsFolder, accountFile, logMessage);
            }
        }
        public string getBalance(string accountName) 
        {
            string accountDir = subClassAppDirectory + "\\accounts\\";
            string accountFile = accountName + ".txt";
            string balance = fileReadSpecific(accountDir, accountFile, 2, 2);
            return balance;
        }
        public string getRecentTransactions(string accountName) 
        {
            string accountDir = subClassAppDirectory + "\\transaction_logs";
            string accountFile = accountName + ".txt";
            //int nLines = fileLinesCheck(accountDir, accountFile);
            string transactions = fileReadAll(accountDir, accountFile);//fileReadSpecific(accountDir, accountFile, nLines - 5, nLines);
            return transactions;
        }
        public bool checkAccountExistence(string accountName) 
        {
            string accountDir = subClassAppDirectory + "\\accounts";
            string accountFile = accountName + ".txt";
            bool exists = fileExistCheck(accountDir, accountFile);
            return exists;
        }

        //Version 1.1 Functions
        public void createAccount(string accountName, string guildID)
        {
            string guildDir = guildsDir + "\\" + correctFileName(guildID);
            string accountDir = guildDir + "\\accounts";
            string accountFile = correctFileName(accountName) + ".txt";

            fileCopy(templateDir, accountTemplate, accountDir, accountFile, false);
        }
        public void accountTransaction(string accountName, string type, int amount, string details, string transactor, string guildID, bool createLog)
        {
            string convertedGuildID = correctFileName(guildID);
            string convertedAccountID = correctFileName(accountName);
            string guildDir = guildsDir + "\\" + convertedGuildID;
            string accountDir = guildDir +  "\\accounts";
            string accountTransactionLogsFolder = guildDir + "\\transaction_logs";
            string accountTransactionLog = accountTransactionLogsFolder + "\\" + convertedAccountID + ".txt";
            string accountFile = convertedAccountID + ".txt";

            if (!checkAccountExistence(convertedAccountID, convertedGuildID))
            {
                createAccount(convertedAccountID, convertedGuildID);
            }

            if (createLog)
            {
                string logMessage = "[" + dateNumber() + "] {Account: " + accountName + "; Transactor: " + transactor + "; Type: " + type + "; Amount: $" + amount + "; Details: " + details + ";}";
                if (fileExistCheck(accountTransactionLog))
                {
                    fileAddLine(accountTransactionLogsFolder, accountFile, logMessage);
                }
                else
                {
                    fileCreate(accountTransactionLogsFolder, accountFile, logMessage);
                }
            }

            int balance = Convert.ToInt32(getBalance(accountName, convertedGuildID));
            if (type == "spend")
            {
                amount = amount * -1;
            }
            int newBalance = balance + amount;
            fileEditExistingLine(accountDir, accountFile, 2, Convert.ToString(newBalance));

        }
        public void transferFunds(string userID, string targetUserID, int amount, string details, string guildID)//need to add implementation of this into bot commands
        {
            //Prepare Filepaths & Names
            string accountTransactionLogsFolder = applicationDirectory + "\\transaction_logs\\" + guildID;
            string transactorAccountTransactionLog = accountTransactionLogsFolder + "\\" + correctFileName(userID) + ".txt";
            string receiverAccountTransactionLog = accountTransactionLogsFolder + "\\" + correctFileName(targetUserID) + ".txt";
            string transactorAccountFile = correctFileName(userID) + ".txt";
            string receiverAccountFile = correctFileName(targetUserID) + ".txt";

            //Perform Transaction
            accountTransaction(correctFileName(userID), "spend", amount, details, userID, guildID, false);
            accountTransaction(correctFileName(targetUserID), "deposit", amount, details, userID, guildID, false);

            //Transaction Logging
            string logMessage = "[" + dateNumber() + "] {Account: " + userID + "; Transactor: " + userID + "; Type: Funds Transfer; Target:" + targetUserID + "Amount: $" + amount + "; Details: " + details + ";}";
            if (fileExistCheck(transactorAccountTransactionLog))
            {
                fileAddLine(accountTransactionLogsFolder, transactorAccountFile, logMessage);
            }
            else
            {
                fileCreate(accountTransactionLogsFolder, transactorAccountFile, logMessage);
            }
            if (fileExistCheck(receiverAccountTransactionLog))
            {
                fileAddLine(accountTransactionLogsFolder, receiverAccountFile, logMessage);
            }
            else
            {
                fileCreate(accountTransactionLogsFolder, receiverAccountFile, logMessage);
            }
        }
        public string getBalance(string accountName, string guildID)
        {
            string convertedGuildID = correctFileName(guildID);
            string convertedAccountID = correctFileName(accountName);
            string accountDir = guildsDir + "\\" + convertedGuildID + "\\accounts";
            string accountFile = convertedAccountID + ".txt";
            string balance = fileReadSpecific(accountDir, accountFile, 2, 2);
            return balance;
        }
        public string getRecentTransactions(string accountName, string guildID)
        {
            string convertedGuildID = correctFileName(guildID);
            string convertedAccountID = correctFileName(accountName);
            string accountTransactionLogsFolder = guildsDir + "\\" + convertedGuildID + "\\transaction_logs";
            string accountFile = convertedAccountID + ".txt";
            string transactions = fileReadAll(accountTransactionLogsFolder, accountFile);
            return transactions;
        }
        public bool checkAccountExistence(string accountName, string guildID)
        {
            string guildDir = guildsDir + "\\" + correctFileName(guildID);
            string accountDir = guildDir + "\\accounts";
            string accountFile = correctFileName(accountName) + ".txt";
            bool exists = fileExistCheck(accountDir, accountFile);
            return exists;
        }
    }
}

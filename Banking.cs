using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileManagementCS;


namespace Banking
{
    internal class Banking
    {
        DateTime dateN;
        FileManagementCS.FILE_MANAGEMENT fm = new();

        public string dateNumber()
        {
            dateN = DateTime.Now;
            string dateNumber = dateN.ToString("yyyy") + dateN.ToString("MM") + dateN.ToString("dd") + dateN.ToString("HH") + dateN.ToString("mm") + dateN.ToString("ss");
            return dateNumber;
        }
        public void createAccount(string accountName)
        {
            string templateDir = fm.applicationDirectory + "\\templates";
            string accountDir = fm.applicationDirectory + "\\accounts";
            string accountFile = accountName + ".txt";

            fm.fileCopy(templateDir, "account.txt", accountDir, accountFile);
        }
        public void accountTransaction(string accountName, string type, int amount, string details, string transactor)
        {
            string accountDir = fm.applicationDirectory + "\\accounts";
            string accountTransactionLogsFolder = fm.applicationDirectory + "\\transaction_logs";
            string accountTransactionLog = accountTransactionLogsFolder + "\\" + accountName + ".txt";
            Console.WriteLine(accountTransactionLog);

            string accountFile = accountName + ".txt";
            if (checkAccountExistence(accountName) == false){
                createAccount(accountName);
            }
            string logMessage = "[" + dateNumber() + "] {Account: " + accountName + "; Transactor: " + transactor + "; Type: " + type + "; Amount: $" + amount + "; Details: " + details + ";}";

            int balance = Convert.ToInt32(getBalance(accountName));
            if(type == "spend")
            {
                amount = amount * -1;
            }
            int newBalance = balance + amount;
            fm.fileEditExistingLine(accountDir, accountFile, 2, Convert.ToString(newBalance));            

            if (File.Exists(accountTransactionLog))
            {
                fm.fileAddLine(accountTransactionLogsFolder, accountFile, logMessage);
            }
            else
            {
                fm.fileCreate(accountTransactionLogsFolder, accountFile, logMessage);
            }         
        }
        public string getBalance(string accountName)
        {
            string accountDir = fm.applicationDirectory + "\\accounts\\";
            string accountFile = accountName + ".txt";
            string balance =fm.fileReadSpecific(accountDir, accountFile, 2, 2);
            return balance;
        }
        public string getRecentTransactions(string accountName) // Gets 5 recent transactions (TBI, currently gets all)
        {
            string accountDir = fm.applicationDirectory + "\\transaction_logs";
            string accountFile = accountName + ".txt";
            //int nLines = fm.fileLinesCheck(accountDir, accountFile);
            string transactions = fm.fileReadAll(accountDir, accountFile);//fm.fileReadSpecific(accountDir, accountFile, nLines - 5, nLines);
            return transactions;
        }
        public bool checkAccountExistence(string accountName)
        {
            string accountDir = fm.applicationDirectory + "\\accounts";
            string accountFile = accountName + ".txt";
            bool exists = fm.fileExistCheck(accountDir, accountFile);
            return exists;
        }
    }
}

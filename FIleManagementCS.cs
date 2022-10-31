﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagementCS
{
    public class FILE_MANAGEMENT
    {
        public string applicationDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);   


        // Major Operations
        public void fileCreate(string filePath, string fileName, string message)
        {
            string pan = filePath + "\\" + fileName;
            string aMessage = message + System.Environment.NewLine;
            byte[] bMessage = System.Text.Encoding.UTF8.GetBytes(aMessage);

            using FileStream file = File.Create(pan);
            long lSize = file.Length;
            long lMSize = bMessage.Length;
            int iSize = Convert.ToInt32(lSize);
            int iMSize = Convert.ToInt32(lMSize);
            file.WriteAsync(bMessage, iSize, iMSize);
            file.Close();
        }
        public void fileCopy(string originPath, string originFileName, string newPath, string newFileName)
        {
            string oPan = originPath + "\\" + originFileName;
            string nPan = newPath + "\\" + newFileName;
            using FileStream fs = File.OpenRead(oPan);
            using var sr = new StreamReader(fs);
            string sContents = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            if (File.Exists(nPan))
            {
                string consoleMessage = newFileName + " already exists, would you like to overwrite?\n1) Yes\n2) No";
                Console.WriteLine(consoleMessage);
                string response = Console.ReadLine();
                switch (response)
                {
                    case "Yes":
                        File.Delete(nPan);
                        fileCreate(newPath, newFileName, sContents);
                        break;
                    case "yes":
                        File.Delete(nPan);
                        fileCreate(newPath, newFileName, sContents);
                        break;
                    case "1":
                        File.Delete(nPan);
                        fileCreate(newPath, newFileName, sContents);
                        break;
                    case "No":
                        Console.WriteLine("Aborting Copy Operation");
                        break;
                    case "no":
                        Console.WriteLine("Aborting Copy Operation");
                        break;
                    case "2":
                        Console.WriteLine("Aborting Copy Operation");
                        break;
                    default:
                        Console.WriteLine("Invalid Response: Aborting Operation");
                        break;
                }
            }
            else
            {
                fileCreate(newPath, newFileName, sContents);
            }
        }

        // Read Operations
        public string fileReadAll(string filePath, string fileName)
        {
            string pan = filePath + "\\" + fileName;
            string message = File.ReadAllText(pan);
            return message;
        }
        public string fileReadSpecific(string filePath, string fileName, int startLine, int endLine)
        {
            string pan = filePath + "\\" + fileName;
            endLine = endLine + 1;
            int readLines = endLine - startLine;

            using FileStream fs = File.OpenRead(pan);
            using var sr = new StreamReader(fs);
            do{
                sr.ReadLine();
                startLine += -1;
            } while (startLine > 1);
            string contents = null;
            do{
                contents = sr.ReadLine();
                readLines += -1;
            } while (readLines > 0);
            sr.Close();
            fs.Close();

            return contents;
        }


        //Edit Operations
        // Currently Bugged
        public void fileEditExistingLine(string filePath, string fileName, int targetLine, string newText)
        {
            string pan = filePath + "\\" + fileName;

            var lines = File.ReadAllLines(pan);   
                    

            string fileFill = "";
            int accounted = 0;
            do
            {
                if(accounted == (targetLine - 1))
                {
                    fileFill += newText + System.Environment.NewLine;
                } else
                {
                    fileFill += lines[accounted] + System.Environment.NewLine;
                }                
                accounted++;
            }while(accounted < lines.Length);
            File.WriteAllText(pan, fileFill);
        }
        public void fileAddLine(string filePath, string fileName, string newText)
        {
            string pan = filePath + "\\" + fileName;
            var lines = File.ReadAllLines(pan);

            string fileFill = null;
            int accounted = 0;
            do
            {
                fileFill += lines[accounted] + System.Environment.NewLine;
                accounted++;
            } while (accounted < lines.Length);
            fileFill += newText + System.Environment.NewLine;
            File.WriteAllText(pan, fileFill);
        }        

        // Get/Check Operations
        public bool fileExistCheck(string originPath, string originFileName)
        {
            string pan = originPath + "\\" + originFileName;
            bool exists;
            exists = File.Exists(pan);
            return exists;
        }
        public int fileLinesCheck(string filePath, string fileFileName)
        {            
            string pan = filePath + "\\" + fileFileName;
            string[] lines;
            lines = File.ReadAllLines(pan);
            int nLines = lines.Length;
            return nLines;
        }
    }
}

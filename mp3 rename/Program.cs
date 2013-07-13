using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileRename
{
    class Program
    {

       static StreamWriter writer = null;

       static void Main(string[] args)
       {
           while (true)
           {
               Console.WriteLine("Please run in admin mode!");
               Console.WriteLine("\tPlease select the option:\n");
               Console.WriteLine("\t\t 1:Remove Leading Digits and .COM's\n");
               Console.WriteLine("\t\t 2:Exact string Replace \n");
               Console.WriteLine("\t\t 3:Exit\n");

               string path = string.Empty;
               int option = 0;
               int count = 0;

               int.TryParse(Console.ReadLine(), out option);
               string filename;
               string directory;

               if (option != 3)
               {
                   Console.WriteLine("Please enter the path:\n");
                   path = Console.ReadLine();
                   writer = File.CreateText(path + "mp3log.txt");
               }

               try
               {

                   switch (option)
                   {
                       case 1:
                           if (!string.IsNullOrWhiteSpace(path))
                           {

                               int i = 0;
                               string filenamePostDelOfDigits = string.Empty;
                               string filenamePostDelOfAd;
                               string finalFilename = string.Empty;
                               bool containsDigits = false;
                               bool containsAd = false;

                               foreach (var item in Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories))
                               {
                                   filename = Path.GetFileName(item);
                                   directory = Path.GetDirectoryName(item);

                                   if (Regex.IsMatch(filename, @"^(\d)+"))
                                   {
                                       i += filename.ToCharArray().TakeWhile(ch => !char.IsLetter(ch)).Count();

                                       filenamePostDelOfDigits = filename.Remove(0, i).ToLower();
                                       finalFilename = filenamePostDelOfDigits;
                                       containsDigits = true;
                                   }

                                   if (filename.Contains(".com"))
                                   {
                                       //clear .COM from the string
                                       int indexOfCOM = filename.IndexOf(".com");
                                       int indexofSpace = filename.LastIndexOf(" ", indexOfCOM, indexOfCOM);
                                       
                                       filenamePostDelOfAd = filename.Remove(indexofSpace,  (indexOfCOM + 4)-indexofSpace);
                                       finalFilename = filenamePostDelOfAd;
                                       containsAd = true;
                                   }

                                   if (containsAd && containsDigits)
                                   {
                                       //clear .COM from the string
                                       int indexOfCOM = filenamePostDelOfDigits.IndexOf(".com");
                                       int indexofSpace = filenamePostDelOfDigits.LastIndexOf(" ", 0, indexOfCOM);
                                       finalFilename = filenamePostDelOfDigits.Remove(indexofSpace, indexOfCOM + 4);
                                      
                                   }
                                   if (containsAd || containsDigits)
                                   {
                                       IsDuplicate(directory,item,finalFilename);
                                   }
                                   containsDigits = false;
                                   containsAd = false;
                                   count++;
                               }

                           }
                           break;
                       case 2:
                           Console.WriteLine("Please enter the string to remove:\n");
                           string replace = Console.ReadLine();

                           if (!string.IsNullOrWhiteSpace(replace))
                           {

                               foreach (var item in Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories))
                               {
                                   filename = Path.GetFileName(item);
                                   directory = Path.GetDirectoryName(item);
                                   if (Regex.IsMatch(filename, replace))
                                   {
                                       IsDuplicate(directory, item, filename.Replace(replace, ""));
                                       count++;
                                   }
                               }

                           }
                           break;
                       case 3:
                           Environment.Exit(0);
                           break;
                       default:
                           Console.WriteLine("Invalid option!\n");
                           break;
                   }
               }
               catch(Exception ex)
               {
                   writer.WriteLine(ex);
               }
               finally
               {
                   writer.Close();
               }

               Console.WriteLine("=============================================================================");
               Console.WriteLine(@"Conversion Complete:joshmachine \m/");
               Console.WriteLine("ConvertedFiles:{0}\n", count);
               Console.WriteLine("=============================================================================");

           }
       }

        static void IsDuplicate(string directory,string oldFilename,string newFilename)
        {

            if (!Directory.GetFiles(directory,newFilename).Any())
                                            {
                                                File.SetAttributes(oldFilename, FileAttributes.Normal);
                                                File.Move(oldFilename, directory + "\\" + newFilename);
                                                writer.WriteLine("Converted filename:" + directory + oldFilename);
                                            }
                                            else
                                            {
                                                File.Delete(directory + "\\" + newFilename);
                                                writer.WriteLine(
                                                    "filename already exists {0},deleting the original file!", oldFilename);
                                            }
        }
    }
}

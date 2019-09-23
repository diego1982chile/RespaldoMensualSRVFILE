using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace RespaldoMensualSRVFILE
{
    class Program
    {
        static List<String> SOURCE_DIRECTORIES = new List<string>();
        static String TARGET_DIRECTORY = @"C:\COPIED FILES\";
        static String ZIP_HOME = @"C:\Program Files\7-Zip\";
        static readonly String ZIP_ARGUMENTS = "a targetName sourceName -mx=9";
        static readonly DateTime TODAY = DateTime.Today;
        static List<String> targetDirectories = new List<string>();


        static void Main(string[] args)
        {
            try
            {
                //Log(TODAY.ToString());
                readParameters();

                // Copiar archivos a disco externo
                
                foreach (String sourceDirectory in SOURCE_DIRECTORIES)
                {
                    string[] tokens = sourceDirectory.Split('\\');
                    String mirror = tokens[tokens.Length - 1];
                    String targetDirectory = TARGET_DIRECTORY + mirror;
                    targetDirectories.Add(targetDirectory);
                    ProcessXcopy(sourceDirectory, TARGET_DIRECTORY);
                }
                

                CompressFiles();
            }
            catch (Exception e)
            {
                //Log(e.Message, EventLogEntryType.Error);
                //Log(e.Message);
                throw e;
            }
        }

        static void readParameters()
        {
            try
            {
                FileStream fileStream = new FileStream("parameters.txt", FileMode.Open);

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    while (reader.Peek() >= 0)
                    {
                        string line = reader.ReadLine();

                        string[] tokens = line.Split('=');

                        if (tokens.Length != 2)
                        {
                            String msg = "Formato no válido. Los parámetros deben ser especificados en la forma: [NOMBRE] = [VALOR]";
                            //Log(msg, EventLogEntryType.Error);
                            throw new System.ApplicationException(msg);
                        }

                        switch (tokens[0])
                        {
                            case "ZIP_HOME":
                                //ZIP_HOME = tokens[1] + "7z.exe";
                                ZIP_HOME += "7z.exe";
                                break;
                            case "SOURCE_DIRECTORIES":
                                string[] tokens2 = tokens[1].Split(',');
                                foreach (String path in tokens2)
                                {
                                    SOURCE_DIRECTORIES.Add(path);
                                }
                                break;
                            default:
                                String msg = "Parámetro no válido. Valores aceptados: ERASER_HOME, PATHS";
                                //Log(msg, EventLogEntryType.Error);
                                //Log(msg);
                                throw new System.ApplicationException(msg);
                        }

                    }
                }
            }
            catch (FileNotFoundException e)
            {
                String msg = "El archivo de parámetros 'parameters.txt' no existe. Debe crear este archivo en la ruta donde se encuentra el ejecutable del aplicativo.";
                //Log(msg, EventLogEntryType.Error);
                //Log(msg);
                throw new System.ApplicationException(msg);
            }
            catch (FormatException e2)
            {
                String msg = "Formato no válido. Los parámetros deben ser especificados en la forma: [NOMBRE] = [VALOR]";
                //Log(msg, EventLogEntryType.Error);
                //Log(msg);
                throw new System.ApplicationException(msg);
            }
        }

        static void ProcessXcopy(string SolutionDirectory, string TargetDirectory)
        {
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo();            
            startInfo.UseShellExecute = false;            
            startInfo.CreateNoWindow = true; //not diplay a windows
            startInfo.Verb = "runas";
            //Give the name as Xcopy
            startInfo.FileName = "xcopy";
            //make the window Hidden
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //Send the Source and destination as Arguments to the process
            startInfo.Arguments = "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" + @" /e /y /I";

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }

            catch (Exception exp)
            {
                throw exp;
            }
        }


        static void CompressFiles()
        {
            string sourceName = "";
            string targetName = " \"" + TARGET_DIRECTORY + TODAY.Year.ToString() + TODAY.Month.ToString() + ".zip" + "\" ";
            foreach (String targetDirectory in targetDirectories)
            {
                sourceName += " \"" + targetDirectory + "\" ";
            }
            //ProcessStartInfo process = new ProcessStartInfo();
            ProcessStartInfo process = new ProcessStartInfo();
            process.FileName = ZIP_HOME;
            process.Arguments = ZIP_ARGUMENTS.Replace("targetName", targetName).Replace("sourceName", sourceName); //argument            
            process.WindowStyle = ProcessWindowStyle.Hidden;                      

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(process))
                {
                    exeProcess.WaitForExit();
                }
            }

            catch (Exception exp)
            {
                throw exp;
            }
        }

    }
}

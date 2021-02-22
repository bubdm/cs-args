using System;

using static System.Console;
using Sprocket.Args;

namespace Sprocket.Args
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Sprocket.Args Examples - Select which example to execute");
            WriteLine("A] Just Options");
            WriteLine("B] Global Option With Default Value");
            WriteLine("C] Multiple Values");
            WriteLine("D] Invalid Multiple Values");
            WriteLine("E] Command With Defaults");
            WriteLine("F] Unspecified Mandatory Option");
            WriteLine("G] Default Command With Default Option");

            char key = Console.ReadKey().KeyChar;

            Console.WriteLine();

            switch (key)
            {
                case 'A':
                case 'a':
                    JustOptions();
                    break;

                case 'B':
                case 'b':
                    GlobalOptionWithDefaultValue();
                    break;

                case 'C':
                case 'c':
                    MultipleValues();
                    break;

                case 'D':
                case 'd':
                    InvalidMultipleValues();
                    break;

                case 'E':
                case 'e':
                    CommandWithDefaultOptionValues();
                    break;

                case 'F':
                case 'f':
                    UnspecifiedMandatoryOption();
                    break;

                case 'G':
                case 'g':
                    DefaultCommandWithDefaultOption();
                    break;
            }
        }

        static void JustOptions()
        {
            Processor p = new();
            p.LoadDefinitionsFromXML(@"<xml>
                    <option name='filename' />
                    <option name='directory' />
                </xml>");

            string inputFromUser = @"filename = me.txt";

            ParseResult res = p.Parse(inputFromUser);

            // ouput global options
            foreach (Option opt in res.GlobalOptions)
            {
                Console.WriteLine($"{opt.Name} : {opt.Value}");
            }

        }

        static void GlobalOptionWithDefaultValue()
        {
            Processor p = new();
            p.LoadDefinitionsFromXML(@"<xml>
                    <option name='filename' default_value='*.csv' />
                    <option name='directory' />
                </xml>");

            string input = @"filename";

            ParseResult res = p.Parse(input);

            // ouput global options
            foreach (Option opt in res.GlobalOptions)
            {
                Console.WriteLine($"{opt.Name} : {opt.Value}");
            }

        }


        static void MultipleValues()
        {
            Processor p = new();
            p.LoadDefinitionsFromXML(@"<xml>
                    <option name='filename' default_value='*.csv' allow_multiple='true' />
                    <option name='directory'  />
                </xml>");

            string input = @"filename=*.csv *.docx directory=C:/bin/ filename ""my file.txt""";

            ParseResult res = p.Parse(input);

            // ouput global options
            foreach (Option opt in res.GlobalOptions)
            {
                Console.Write(opt.Name);
                Console.Write(": ");
                foreach (string value in opt.Values)
                {
                    Console.Write(value);
                    Console.Write("; ");
                }
                Console.WriteLine();
            }
        }

        static void InvalidMultipleValues()
        {
            Processor p = new();
            p.LoadDefinitionsFromXML(@"<xml>
                    <option name='filename' default_value='*.csv' />
                    <option name='directory'  />
                </xml>");

            string input = @"filename=*.csv *.docx directory=C:/bin/";

            ParseResult res = p.Parse(input);

            // ouput global options
            foreach (Option opt in res.GlobalOptions)
            {
                Console.Write(opt.Name);
                Console.Write(": ");
                foreach (string value in opt.Values)
                {
                    Console.Write(value);
                    Console.Write("; ");
                }
                Console.WriteLine();
            }
        }

        static void CommandWithDefaultOptionValues()
        {
            Processor p = new();
            p.LoadDefinitionsFromXML(@"<xml>
                    <command name='delete'>
                        <option name='filespec' />
                        <option name='-confirm' default_value='yes' />
                    </command>
                    <command name='restore'>
                        <option name='filespec' />
                        <option name='-backup' default_value='no' />
                    </command>
                    <option name='--language' />
                </xml>");

            string input = @"delete --language=ENG";

            ParseResult res = p.Parse(input);

            // output commands and their options
            foreach (Command cmd in res.Commands)
            {
                Console.WriteLine($"Command: {cmd.Name}");
                if (cmd.Options.Count > 0)
                {
                    foreach (Option opt in cmd.Options)
                    {
                        Console.Write($"    Option:{opt.Name} : ");
                        foreach (string value in opt.Values)
                        {
                            Console.Write(value);
                            Console.Write("; ");
                        }
                        Console.WriteLine();
                    }
                }
            }

            // ouput global options
            foreach (Option opt in res.GlobalOptions)
            {
                Console.Write($"Global Option: {opt.Name} : ");
                foreach (string value in opt.Values)
                {
                    Console.Write(value);
                    Console.Write("; ");
                }
                Console.WriteLine();
            }
        }

        static void UnspecifiedMandatoryOption()
        {
            Processor p = new();
            p.LoadDefinitionsFromXML(@"<xml>
                    <command name='delete'>
                        <option name='filespec' required='true' />
                        <option name='-confirm' default_value='yes' />
                    </command>
                    <command name='restore'>
                        <option name='filespec' />
                        <option name='-backup' default_value='no' />
                    </command>
                    <option name='--language' />
                </xml>");

            string input = @"delete --language=ENG";

            ParseResult res = p.Parse(input);
        }

        static void DefaultCommandWithDefaultOption()
        {
            Processor p = new();
            p.LoadDefinitionsFromXML(@"<xml>
                    <command name='delete' is_default='true' >
                        <option name='filespec' required='true' default_value = '*.txt' />
                        <option name='-confirm' default_value='yes' />
                    </command>
                    <command name='restore'>
                        <option name='filespec' />
                        <option name='-backup' default_value='no' />
                    </command>
                    <option name='--language' />
                </xml>");

            string input = "";

            ParseResult res = p.Parse(input);

            // output commands and their options
            foreach (Command cmd in res.Commands)
            {
                Console.WriteLine($"Command: {cmd.Name}");
                if (cmd.Options.Count > 0)
                {
                    foreach (Option opt in cmd.Options)
                    {
                        Console.Write($"    Option:{opt.Name} : ");
                        foreach (string value in opt.Values)
                        {
                            Console.Write(value);
                            Console.Write("; ");
                        }
                        Console.WriteLine();
                    }
                }
            }

            // ouput global options
            foreach (Option opt in res.GlobalOptions)
            {
                Console.Write($"Global Option: {opt.Name} : ");
                foreach (string value in opt.Values)
                {
                    Console.Write(value);
                    Console.Write("; ");
                }
                Console.WriteLine();
            }
        }

    }
}

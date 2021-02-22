# Sprocket.Args.dll v1.0.0 API documentation

Created by [David Pullin](https://ict-man.me)

Readme Last Updated on 22.02.2021

The full API with further examples can be found at https://ict-man.me/sprocket/api/Sprocket.Args.html

Licence GPL-3.0 


## Summary

Args is a command line argument parser.  It aims to be simple to configure and forgiving when parsing input.

Configuration is performed via XML. Both the configuration and parsed results contain two main objects: **Commands** and **Options**.

- An Option is a word to indicate additional information.  All Options have a value.

- A Command is a word to represent the action to take.

- Commands can have multiple Options.

- Options can hold multiple values

- Options can be global as well as specific to a Command.

- A default Command can be specified.

- An Option can have a default value.

- Options can be delcared as mandatory.

- Option values can be quoted (single or double) to enclose spaces or other text that should be parsed as the value not as a keyword.



## Example Overview

    static void Main(string[] args)
    {
	  string xml = @"
        <xml>
            <command name='list' allow_multiple='true'>
                <option name='file' required='true' default_value='*.csv' allow_multiple='true' />
            </command>
            <command name='map' allow_multiple='true'>
                <option name='file' required='true' default_value='*.csv' allow_multiple='true' />
                <option name='overwrite' is_flag='true' />
                <option name='output-file' />
            </command>
            <option name='--path' />
            <option name='--nologo' is_flag='true' />
        </xml>";

		Processor p = new();
		try
		{
			p.LoadDefinitionsFromXML(xml);
			ParseResult res = p.Parse(string.Join(" ", args));
			System.Console.WriteLine(res.ToJson(true));
		}
		catch (System.Exception e)
		{
			System.Console.WriteLine(e.Message);
		}
	}

If the command line supplied was **map filespec=\*.txt \*.csv  overwrite list filespec --nologo list \*.txt**

Then the res.**Commands** collection would contain 3 Command objects.

 - *map*
 - *list*
 - *list*

 *list* is present twice as it was entered twice by the user.  The XML definition allowed this command to be present multiple times. The order of the Command objects in the Commands collection are in the order they appeared in the supplied command line.

 This is a shortened version of the JSON representation of **res.ToJson(true);**

 	{   "Commands": [
		{   
            "Name": "map",
		    "Options": [
                {   "Name": "filespec", "Values": [ "*.txt", "*.csv" ] },
                {   "Name": "overwrite", "Values": ["true"] }
            ]
		},{
		  "Name": "list",
		  "Options": [
			    {   "Name": "filespec", "Values": [ "*.csv" ] }
          ]
		},{
		  "Name": "list",
		  "Options": [
			    {   "Name": "filespec", "Values": [	"*.txt" ],
		  ]
		}
	  ],
	  "GlobalOptions": [
		{
		  "Name": "--nologo", "Values": ["true"]
		}
	  ]
	}

 - *map* - the first parsed command, parsed two options.

    - The first option 'filespec' has two values.  This option was defined in the XML as allowing multiple options.

    - The second option, whilst no value was specified in the command line, it has the value "true".  In the XML this was defined as a flag field.  As such no value is expected.  When used flag options will always have the value "true" (string).
 
 - *list* - the second parsed command, parsed one option

    - The option 'filespec' has been given the value of *.cvs as no value was given so the default was used.  

    - In the command line the word following list was *--nologo*.  As this matched a with a keyword (in this case an Option Name) and the option 'filespec' had a default, the keyword took presidence.

 - *last* - the third parsed command, parsed one option

    - The option value here is '*.txt' as supplied by the user.

    - Note that this command appeared after *--nologo*.  This is to highlight that Global Options do not necessary have to be at the end.

 - The Global Option's collection contains one item called *--nologo*.  Again, this was defined in the XML as a flag field, as such its value will default to "true".


## XML-Format

The example below defined three Commands.  Command can have 0 or more Options that are only valid for that Commmand.  The command *reset* has no options.

Options not within a Command and called Global Options.

    <xml>
        <command name='list' allow_multiple='true'>
            <option name='file' required='true' default_value='*.csv' allow_multiple='true' />
        </command>
        <command name='map' allow_multiple='true'>
            <option name='file' required='true' default_value='*.csv' allow_multiple='true' />
            <option name='overwrite' is_flag='true' />
            <option name='output-file' />
        </command>
        <command name='reset' />
        <option name='--path' />
        <option name='--nologo' is_flag='true' />
    </xml>";

<br>

## Command With A Default Option Value

This example show a Command with Options where some of them have default values.  

This example also shows some of the Options name's prefixed with '-' and '--'.  These have no special meaning but is shown here to highlight that, depending on your preference, you can have names with non-alpha characters.

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

Expected Ouput:

Although, the **-confirm** Option was not specified in the command line, it is still returned in the result set as it has a default value.

    Command: delete
        Option:-confirm : yes; 
    Global Option: --language : ENG;

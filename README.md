# Sprocket.Args.dll v1.0.0 API documentation

Created by [David Pullin](https://ict-man.me)

Readme Last Updated on 22.02.2021

The full API can be found at https://ict-man.me/sprocket/api/Sprocket.Args.html

Licence GPL-3.0 

<br>

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


<br>

## Contents

|Item|Details|
|--|--|
|[An Example](#An-Example)|A demostration of XML markup and a breakdown of the Command and Options parsed from an example command line.|
|[XML Format](#XML-Format)|Details of options to described allows Command and Options|
|[Just Options](#Example:-Just-Options)|Example of use with just Global Options being used. No Commands.|
|[Global Option With Default Value](#Example-Global-Option-With-Default-Value)|Example of a Global Option with a default value.|
|[Option With Multiple Values](#Example-Option-With-Multiple-Values)|Example of an Option where multiple values are allowed.|
|[Command With A Default Option Value](#Command-With-A-Default-Option-Value)|Example of using commands where the Command entered has an Option with a default value.
|[Command With Unspecified Required Option](#Command-With-An-Unspecified-Required-Option)|Example of a Command that has a required Option that was not specified on the command line.|
|[Default Command](#A-Default-Command-with-a-Default-Value)|Example of a Command that has been set as the default.|
|[Notes](#Notes)|Additional Notes and Comments.|
|[Future Improvements](#Future-Improvements)|Ideas for future improvements.|
<br>

<br>

## An Example

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


<br>

<br>

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

### \<Command\> Node

The Command Node defines an available Command.  Attributes are used to configure a Command.

| Attribute | Required | Description | Default |
|--|--|--|--|
|name| yes | A unique name for the command.| |
|allow_multiple||Set to "true" if the command can be specified more than once in the command line. |false|
|is_default||Set to "true" if this should be the default command.  If no command is specified then this command will be returned in the parsed results.  Only one Command can be set as default.|false|


<br>

### \<Option\> Node

The Option Node defines an option for a Command or a Global Option.  Attributes are used to configure an Option.

| Attribute | Required | Description | Default |
|--|--|--|--|
|name|yes|The name of the option.||
|allow_multiple||Set to "true" to allow multiple values to be specified for this option.|false|
|default_value||The value to assign this option if not value is supplied||
|is_flag||Set to "true" to indicate that this Option is a flag.  As such no value is expected or allowed to be supplied.  When a flag Option is present in the command line the parser will set the value of this option as "true" (string).
|required||Set to true if the option is required.  This attribute can only be set against Options belonging to a Command.|false|

<br>

## Example Just Options

In this example, we are not using Commands. Useful for programs where only one action is taking place and the user can specify various options to affect that behaviour.

    Processor p = new();
    p.LoadDefinitionsFromXML(@"<xml>
            <option name='filename' />
            <option name='directory' />
        </xml>");

    string input = @"filename = me.txt";

    ParseResult res = p.Parse(input);

    // ouput global options
    foreach(Option opt in res.GlobalOptions) {
        Console.WriteLine($"{opt.Name} : {opt.Value}");
    }

Expected Ouput:

    filename : me.txt

<br>

## Example Global Option With Default Value

In this example, the option **filename** has been supplied in the command line but no value.  The XML here defines a default value to be used.

    Processor p = new();
    p.LoadDefinitionsFromXML(@"<xml>
            <option name='filename' default_value='*.csv' />
            <option name='directory' />
        </xml>");

    string input = @"filename";

    ParseResult res = p.Parse(input);

    // ouput global option
    foreach (Option opt in res.GlobalOptions)
    {
        Console.WriteLine($"{opt.Name} : {opt.Value}");
    }

Expected Ouput:

    filename : *.csv


<br>

## Example Option With Multiple Values

This example shows an option where multiple values are allowed.  Here, the option **filename** not only has been given two values at once, but the option itself is also repeated.

This example also highlights the use of quotes around an Option's value and also shows the useoptional use of the equals sign.

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
        foreach(string value in opt.Values) {
            Console.Write(value);
            Console.Write("; ");
        }
        Console.WriteLine();
    }


Expected Ouput:

    filename: *.csv; *.docx; my file.txt; 
    directory: C:/bin/;


<br>

## Example With Invalid Multiple Values

This example is the same as above but where **filename** is not allowing multiple values.

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
        foreach(string value in opt.Values) {
            Console.Write(value);
            Console.Write("; ");
        }
        Console.WriteLine();
    }


Expected Ouput:

An exception is raised when parsing the string when it encouters **.docx*.  This is due to that Command not allowing multiple values. Given that *.docx* does not match any other known Command name or Option name an InvalidCommandException is raised.

    Unhandled exception. Sprocket.Args.InvalidCommandException: Invalid command '*.docx'


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

<br>

## Command With An Unspecified Required Option

In this example, a slight change has been made to the XML from above.  The filespec option has now been flagged as required.

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

Expected Ouput:

The Option **filespec** is a required option when the **delete** Command is usespecified.  Given that **filespec** has no default value and no value was supplied a MissingValueException is raised.

    Unhandled exception. Sprocket.Args.MissingValueException: No value specified for option 'filespec' for command 'delete'

<br>

## A Default Command with a Default Value

This example shows a default Command with Options that have default values.  The input string being processed is empty.

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

Expected Ouput:

Having a default Command is useful for when your application is run without any parameters.

    Command: delete
        Option:filespec : *.txt;
        Option:-confirm : yes;


<br>

## Notes

 - Option Values are not validated by the parser.  This is up to your calling code.

 - The full API can be found at https://ict-man.me/sprocket/api/Sprocket.Args.html

 
<br>


## Future Improvements

Ideas for improvements always welcome.  Possible future improvement could include:

- Value validation.

- Automatic help generation.

using System;
using System.Text;

#nullable enable

namespace Sprocket.Args
{
    /// <summary>
    /// Parses user input (e.g. the command line arguments provided) against the Commands and Options
    /// previously defined via XML.  See <see cref="Sprocket.Args.Processor.LoadDefinitionsFromXML(string)"/>.
    /// 
    /// This class is internal to prevent external use to avoid issues in the future if the internal 
    /// logic is changed.
    /// </summary>
    internal class Parser
    {
        private string text;
        private CommandDefCollection cmdDefs;
        private OptionDefCollection globalOptDefs;

        public Parser(CommandDefCollection cmdDefs, OptionDefCollection globalOptDefs, string text)
        {
            this.text = text;
            this.cmdDefs = cmdDefs;
            this.globalOptDefs = globalOptDefs;
        }

        public ParseResult Parse()
        {
            IntermediateCommandResultCollection icrc;

            ParsingInfo pi = new();
            icrc = this.ParseOuter(new TextToParse(this.text), 0, pi);

            //Transform IntermediateCommandResultCollection into CommandCollection
            CommandCollection resultCommands = new();
            foreach (IntermediateCommandResult icr in icrc)
            {
                if (icr.Cmd != null)
                {
                    resultCommands.Add(icr.Cmd);
                }
            }

            //If command collection is empty then add default commands
            if (0 == resultCommands.Count)
            {
                foreach (CommandDef cmdDef in this.cmdDefs)
                {
                    if (cmdDef.IsDefault)
                    {
                        resultCommands.Add(new Command(cmdDef.Name));
                    }
                }
            }


            OptionCollection globalOptions = new();

            if (resultCommands.Count > 0)
            {
                bool anyGlobalOptionsDetected = false;

                //Add default values to found commands for options not specified
                foreach (Command cmd in resultCommands)
                {
                    if (cmd.Name.Equals("##global", StringComparison.OrdinalIgnoreCase))
                    {
                        anyGlobalOptionsDetected = true;
                    }
                    else
                    {
                        CommandDef cmdDef = this.cmdDefs[cmd.Name.ToUpperInvariant()];

                        foreach (OptionDef optDef in cmdDef.OptionsDefs)
                        {
                            if (!string.IsNullOrEmpty(optDef.DefaultValue))
                            {
                                if (0 == cmd.Options.CountWithName(optDef.Name))
                                {

                                    cmd.AddOption(new Option(optDef.Name, optDef.DefaultValue));

                                }
                            }
                        }
                    }

                }

                //Check for commands missing mandatory options            
                //Add default values to found commands for options not specified
                foreach (Command cmd in resultCommands)
                {
                    if (cmd.Name.Equals("##global", StringComparison.OrdinalIgnoreCase))
                    {
                        anyGlobalOptionsDetected = true;
                    }
                    else
                    {
                        CommandDef cmdDef = this.cmdDefs[cmd.Name.ToUpperInvariant()];
                        foreach (OptionDef optDef in cmdDef.OptionsDefs)
                        {
                            if (optDef.Required && 0 == cmd.Options.CountWithName(optDef.Name))
                            {
                                throw new MissingValueException(cmdDef.Name, optDef.Name);
                            }
                        }
                    }
                }

                // if ##global detected then move the options within to optionarray to remove the ##global
                if (anyGlobalOptionsDetected)
                {

                    CommandCollection newCommands = new();

                    foreach (Command cmd in resultCommands)
                    {
                        if (cmd.Name.Equals("##global", StringComparison.OrdinalIgnoreCase))
                        {
                            foreach (Option opt in cmd.Options)
                            {
                                globalOptions.Add(opt);
                            }

                        }
                        else
                        {
                            newCommands.Add(cmd);
                        }

                        resultCommands = newCommands;

                    }
                }
            }

            return new ParseResult(resultCommands, globalOptions);

        }

        private IntermediateCommandResultCollection ParseOuter(TextToParse ttp, int recurseLevel, ParsingInfo pi)
        {
            // IntermediateCommandResultCollection ir = this.ParseInner(new TextToParse(this.text), recurseLevel, pi);
            IntermediateCommandResultCollection icrc = this.ParseInner(ttp, recurseLevel, pi);


            //merge entries multiple unprocessed text together
            IntermediateCommandResult? lastICRWithUnprocessedText = null;
            bool anyUnprocessed = false;
            IntermediateCommandResultCollection tempICRC = new();

            foreach (IntermediateCommandResult icr in icrc)
            {
                if (icr.UnprocessedText != null)
                {
                    anyUnprocessed = true;

                    if (lastICRWithUnprocessedText != null)
                    {

                        //append the text of this ICR to lastICRWithUnprocessedText
                        lastICRWithUnprocessedText.UnprocessedText += " " + icr.UnprocessedText;

                    }
                    else
                    {
                        lastICRWithUnprocessedText = icr;
                        tempICRC.Add(icr);
                    }
                }
                else
                {
                    lastICRWithUnprocessedText = null;
                    tempICRC.Add(icr);
                }
            }

            // Was there any unprocess text after the first parsing attempt?
            // This can happen if input/entered command line arguments did not match a Command or Global Option
            // On the first pass, such text, is placed as UnprocessedText into the ICR object
            if (0 == recurseLevel && anyUnprocessed)
            {
                // Send ICR unprocess text back for parsing, but this time,
                // a Default Command and Options can be applied if no Command or Option (as appropiate)
                // was found on the first pass.
                IntermediateCommandResultCollection newICRC = new();
                foreach (IntermediateCommandResult icr in tempICRC)
                {
                    if (icr.UnprocessedText != null)
                    {
                        IntermediateCommandResultCollection ir2 = this.ParseOuter(new TextToParse(icr.UnprocessedText), recurseLevel + 1, pi);

                        foreach (IntermediateCommandResult icr2 in ir2)
                        {
                            newICRC.Add(icr2);
                        }
                    }
                    else
                    {
                        newICRC.Add(icr);
                    }
                }
                return newICRC;

            }
            else
            {
                return tempICRC;
            }
        }

        private IntermediateCommandResultCollection ParseInner(TextToParse ttp, int recurseLevel, ParsingInfo pi)
        {
            IntermediateCommandResultCollection res = new();

            CommandAndOptionDetails? currentCmdAndOpt = null;

            while (!ttp.isAtEnd())
            {
                WordDetails currentWord = ttp.GetNextWord();

                // is this a global option
                OptionDef? qualOptDef;
                if (!currentWord.WasQuoted && (qualOptDef = this.FindQualifyingGlobalOption(currentWord.Text, pi, res)) != null)
                {
                    //were we waiting for a value for another option
                    if (null != currentCmdAndOpt && null != currentCmdAndOpt.currentOption && currentCmdAndOpt.currentOption.IsAwaitingFirstValue)
                    {
                        if (currentCmdAndOpt.currentOption.OptDef.Required)
                        {
                            if (null != currentCmdAndOpt.currentOption.OptDef.DefaultValue)
                            {
                                currentCmdAndOpt.currentOption.Opt.Values.Clear();
                                currentCmdAndOpt.currentOption.Opt.Values.Add(currentCmdAndOpt.currentOption.OptDef.DefaultValue);
                                currentCmdAndOpt.currentOption.IsAwaitingFirstValue = false;
                            }
                            else
                            {
                                throw new MissingValueException(currentCmdAndOpt.CmdDef.Name, currentCmdAndOpt.currentOption.OptDef.Name);
                            }
                        }
                    }

                    string use_value = string.Empty;
                    bool use_IsAwaitingFirstValue = true;

                    // this is a flag, no value expected, default to 'true' or use default value if specified
                    if (qualOptDef.IsFlag)
                    {
                        use_value = (null != qualOptDef.DefaultValue) ? qualOptDef.DefaultValue : "true";
                        use_IsAwaitingFirstValue = false;
                    }

                    var newOption = new Option(qualOptDef.Name, use_value);

                    currentCmdAndOpt = new CommandAndOptionDetails(
                                        new CommandDef("##global", false, qualOptDef.AllowMultiple),
                                        new Command("##global"),
                                        new OptionDetails
                                        {
                                            OptDef = qualOptDef,
                                            Opt = newOption,
                                            IsAwaitingFirstValue = use_IsAwaitingFirstValue
                                        }
                                        );
                    currentCmdAndOpt.Cmd.AddOption(newOption);
                    res.Add(IntermediateCommandResult.CreateAsCommand(currentCmdAndOpt.Cmd));
                    continue;
                }


                if ((currentCmdAndOpt?.currentOption?.IsAwaitingFirstValue ?? false) == false && isThisACommandAndNotYetUsedOrAllowsMultiples(currentWord.Text, pi))
                {
                    currentCmdAndOpt = new CommandAndOptionDetails(
                        this.cmdDefs[currentWord.TextUpper],
                        new Command(this.cmdDefs[currentWord.TextUpper].Name),
                        null
                    );

                    res.Add(IntermediateCommandResult.CreateAsCommand(currentCmdAndOpt.Cmd));
                    if (!pi.commandsFound.Contains(currentCmdAndOpt.CmdDef.Name.ToUpperInvariant()))
                    {
                        pi.commandsFound.Add(currentCmdAndOpt.CmdDef);

                    }
                    continue;
                }
                else
                {
                    if (null != currentCmdAndOpt && null != currentCmdAndOpt.currentOption)
                    {
                        //does this value match with an option name for the current command
                        if (currentCmdAndOpt.CmdDef.OptionsDefs.Contains(currentWord.TextUpper))
                        {
                            //will currentOption be left without a value
                            if (currentCmdAndOpt.currentOption.OptDef.Required && currentCmdAndOpt.currentOption.IsAwaitingFirstValue)
                            {
                                throw new MissingValueException(currentCmdAndOpt.CmdDef.Name, currentCmdAndOpt.currentOption.OptDef.Name);
                            }

                            if (!currentCmdAndOpt.currentOption.IsAwaitingFirstValue)
                            {
                                // have we already come across this option
                                Option? isThisExistingOptionInCurrentCommand = currentCmdAndOpt.Cmd.Options[currentWord.Text];
                                if (null == isThisExistingOptionInCurrentCommand)
                                {
                                    //option not not already used for current command

                                    string use_value = string.Empty;
                                    bool use_IsAwaitingFirstValue = true;

                                    // this is a flag, no value expected, default to 'true' or use default value if specified
                                    OptionDef use_OptDef = currentCmdAndOpt.CmdDef.OptionsDefs[currentWord.TextUpper];
                                    if (use_OptDef.IsFlag)
                                    {
                                        use_value = (null != use_OptDef.DefaultValue) ? use_OptDef.DefaultValue : "true";
                                        use_IsAwaitingFirstValue = false;
                                    }



                                    var newOption = new Option(currentWord.Text, use_value);
                                    currentCmdAndOpt = new CommandAndOptionDetails(
                                        currentCmdAndOpt.CmdDef,
                                        currentCmdAndOpt.Cmd,
                                        new OptionDetails
                                        {
                                            OptDef = currentCmdAndOpt.CmdDef.OptionsDefs[currentWord.TextUpper],
                                            Opt = newOption,
                                            IsAwaitingFirstValue = use_IsAwaitingFirstValue
                                        }
                                    );

                                    currentCmdAndOpt.Cmd.AddOption(newOption);

                                    continue;
                                }
                                else
                                {
                                    // Already come across the use of this option for the current command
                                    // Add another value if the option allows multiple values
                                    var usedOptDef = currentCmdAndOpt.CmdDef.OptionsDefs[currentWord.TextUpper];
                                    if (usedOptDef.AllowMultiple)
                                    {
                                        // is this command (currentWord) the one we are currently tracking?
                                        if (currentCmdAndOpt.currentOption.OptDef.Name == currentWord.Text)
                                        {
                                            // continue and look for a value
                                            continue;
                                        }
                                        else
                                        {
                                            // find previous parsed option in current command
                                            // and set back as the current option
                                            currentCmdAndOpt = new CommandAndOptionDetails(
                                                currentCmdAndOpt.CmdDef,
                                                currentCmdAndOpt.Cmd,
                                                new OptionDetails
                                                {
                                                    OptDef = currentCmdAndOpt.CmdDef.OptionsDefs[currentWord.TextUpper],
                                                    Opt = isThisExistingOptionInCurrentCommand,
                                                    IsAwaitingFirstValue = false
                                                }
                                            );

                                            continue;


                                        }
                                    }
                                    else
                                    {
                                        throw new DuplicateOptionException(currentCmdAndOpt.CmdDef.Name, currentWord.Text);
                                    }
                                }

                            }
                        }


                        if (currentCmdAndOpt.currentOption.IsAwaitingFirstValue)
                        {
                            //assign this as a value to the current option
                            currentCmdAndOpt.currentOption.Opt.Values.Clear();
                            currentCmdAndOpt.currentOption.Opt.Values.Add(currentWord.Text);
                            currentCmdAndOpt.currentOption.IsAwaitingFirstValue = false;
                            continue;
                        }
                        else
                        {
                            // word is not a command, nor a globl option keyword nor are we waiting for a value for the currentOpt 
                            // however, if currentOptDef allows multiple values add this word as another value
                            if (currentCmdAndOpt.currentOption.OptDef.AllowMultiple)
                            {
                                currentCmdAndOpt.currentOption.Opt.Values.Add(currentWord.Text);
                                continue;
                            }
                            else
                            {
                                // word is not a command, nor an option keyword nor are we waiting for a value for the currentOpt
                                // The currentOptionDef does not allow multiples values, so check if there another option for the currentCmd

                                var optDef = this.GetUnusedOptionInCommand(currentCmdAndOpt.Cmd, currentCmdAndOpt.CmdDef);

                                if (optDef == null)
                                {
                                    // if command used before then throw
                                    if (pi.commandsFound.Contains(currentWord.TextUpper))
                                    {
                                        throw new DuplicateCommandException(currentWord.Text);
                                    }
                                    else
                                    {
                                        throw new InvalidCommandException(currentWord.Text);
                                    }
                                }
                                else
                                {
                                    // there is another option for the current command that is not used - assume this one
                                    var newOption = new Option(optDef.Name, currentWord.Text);
                                    currentCmdAndOpt = new CommandAndOptionDetails(
                                        currentCmdAndOpt.CmdDef,
                                        currentCmdAndOpt.Cmd,
                                        new OptionDetails
                                        {
                                            OptDef = optDef,
                                            Opt = newOption,
                                            IsAwaitingFirstValue = false
                                        }
                                    );

                                    currentCmdAndOpt.Cmd.AddOption(newOption);

                                    continue;
                                }
                            }
                        }
                    }
                }

                if (null == currentCmdAndOpt)
                {
                    // if here no current command, and word being processed does not match with a known command

                    if (recurseLevel > 0)
                    {
                        // try to assign currentWord as the first value in unused command where is_default = true
                        CommandDef? unusedCmdDef = this.GetFirstUnusedCommandWhereIsDefaultIsSet(currentWord.Text, pi);

                        if (null == unusedCmdDef)
                        {
                            throw new InvalidCommandException(currentWord.Text);
                        }

                        var optDef = unusedCmdDef.OptionsDefs[0];
                        var newOption = new Option(optDef.Name, currentWord.Text);

                        currentCmdAndOpt = new CommandAndOptionDetails(
                            unusedCmdDef,
                            new Command(unusedCmdDef.Name),
                            new OptionDetails
                            {
                                OptDef = optDef,
                                Opt = newOption,
                                IsAwaitingFirstValue = false
                            }
                        );

                        currentCmdAndOpt.Cmd.AddOption(newOption);

                        res.Add(IntermediateCommandResult.CreateAsCommand(currentCmdAndOpt.Cmd));
                        pi.commandsFound.Add(unusedCmdDef);
                    }
                    else
                    {
                        // Record the current word as 'CreateAsUnProcessedText' which may be 
                        // checked again via ParseOuter's self recursion.
                        if (currentWord.WasQuoted)
                        {
                            res.Add(IntermediateCommandResult.CreateAsUnProcessedText(currentWord.QuotedChar + currentWord.Text + currentWord.QuotedChar));
                        }
                        else
                        {
                            res.Add(IntermediateCommandResult.CreateAsUnProcessedText(currentWord.Text));
                        }

                    }
                }
                else
                {
                    // is this the name of an option for the current command
                    if (!currentWord.WasQuoted && currentCmdAndOpt.CmdDef.OptionsDefs.Contains(currentWord.TextUpper))
                    {

                        string use_value = string.Empty;
                        bool use_IsAwaitingFirstValue = true;

                        // this is a flag, no value expected, default to 'true' or use default value if specified
                        OptionDef use_OptDef = currentCmdAndOpt.CmdDef.OptionsDefs[currentWord.TextUpper];
                        if (use_OptDef.IsFlag)
                        {
                            use_value = (null != use_OptDef.DefaultValue) ? use_OptDef.DefaultValue : "true";
                            use_IsAwaitingFirstValue = false;
                        }


                        var newOption = new Option(currentWord.Text, use_value);

                        currentCmdAndOpt = new CommandAndOptionDetails(
                            currentCmdAndOpt.CmdDef,
                            currentCmdAndOpt.Cmd,
                            new OptionDetails
                            {
                                OptDef = use_OptDef,
                                Opt = newOption,
                                IsAwaitingFirstValue = use_IsAwaitingFirstValue
                            }
                        );

                        currentCmdAndOpt.Cmd.AddOption(newOption);
                    }
                    else
                    {
                        OptionDef? optDef = this.GetUnusedOptionInCommand(currentCmdAndOpt.Cmd, currentCmdAndOpt.CmdDef);

                        if (optDef != null)
                        {
                            // Add new option to existing command
                            var newOption = new Option(optDef.Name, currentWord.Text);

                            currentCmdAndOpt = new CommandAndOptionDetails(
                                currentCmdAndOpt.CmdDef,
                                currentCmdAndOpt.Cmd,
                                new OptionDetails
                                {
                                    OptDef = optDef,
                                    Opt = newOption,
                                    IsAwaitingFirstValue = false
                                }
                            );

                            currentCmdAndOpt.Cmd.AddOption(newOption);
                        }
                    }

                }
            } //end of text

            // were we awaiting a value for an option at the time we reached end of string
            if (currentCmdAndOpt?.currentOption?.IsAwaitingFirstValue ?? false)
            {

                // is there a default value for the option?
                if (null != currentCmdAndOpt.currentOption.OptDef.DefaultValue)
                {
                    currentCmdAndOpt.currentOption.Opt.Values.Clear();
                    currentCmdAndOpt.currentOption.Opt.Values.Add(currentCmdAndOpt.currentOption.OptDef.DefaultValue);

                    currentCmdAndOpt = new CommandAndOptionDetails(
                            currentCmdAndOpt.CmdDef,
                            currentCmdAndOpt.Cmd,
                            new OptionDetails
                            {
                                OptDef = currentCmdAndOpt.currentOption.OptDef,
                                Opt = currentCmdAndOpt.currentOption.Opt,
                                IsAwaitingFirstValue = false
                            }
                    );
                }
                else
                {
                    throw new MissingValueException(currentCmdAndOpt.CmdDef.Name, currentCmdAndOpt.currentOption.OptDef.Name);
                }

            }

            return res;
        }

        private OptionDef? GetUnusedOptionInCommand(Command cmd, CommandDef cmdDef)
        {
            for (int idx = 0; idx < cmdDef.OptionsDefs.Count; idx++)
            {
                OptionDef optDef = cmdDef.OptionsDefs[idx];

                //has this option not been used or are multiples are allowed?
                if (0 == cmd.Options.CountWithName(optDef.Name) || optDef.AllowMultiple)
                {
                    return optDef;
                }
            }

            return null;
        }

        private CommandDef? GetFirstUnusedCommandWhereIsDefaultIsSet(string word, ParsingInfo pi)
        {
            for (int idx = 0; idx < this.cmdDefs.Count; idx++)
            {
                var cmdDef = this.cmdDefs[idx];

                if (cmdDef.IsDefault && !pi.commandsFound.Contains(cmdDef.Name.ToUpperInvariant()))
                {
                    return cmdDef;
                }

            }

            return null;
        }

        private OptionDef? FindQualifyingGlobalOption(string word, ParsingInfo pi, IntermediateCommandResultCollection icrc)
        {
            foreach (var optDef in this.globalOptDefs)
            {
                if (optDef.Name.Equals(word, StringComparison.OrdinalIgnoreCase))
                {
                    if (!optDef.AllowMultiple)
                    {
                        // check that we have not already used this option in ##global
                        foreach (IntermediateCommandResult icr in icrc)
                        {
                            // is this a command
                            if (icr.Cmd != null)
                            {
                                // is it the pseudo global command
                                if (icr.Cmd.Name.Equals("##global", StringComparison.OrdinalIgnoreCase))
                                {
                                    // have we used this option before
                                    if (icr.Cmd.Options != null)
                                    {
                                        foreach (Option opt in icr.Cmd.Options)
                                        {
                                            if (opt.Name.Equals(word, StringComparison.OrdinalIgnoreCase))
                                            {
                                                throw new DuplicateOptionException("", word);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return optDef;

                }

            }
            return null;
        }

        private bool isThisACommandAndNotYetUsedOrAllowsMultiples(string word, ParsingInfo pi)
        {
            //Is this word a command
            if (this.cmdDefs.Contains(word.ToUpperInvariant()))
            {

                //Have we already processed this command?
                if (pi.commandsFound.Contains(word.ToUpperInvariant()))
                {
                    CommandDef cmdDef = this.cmdDefs[word.ToUpperInvariant()];

                    //Although already processed, if it is allowed multiple times
                    if (cmdDef.AllowMultiple)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


        private record OptionDetails
        {
            public OptionDef OptDef { get; init; } = new();
            public Option Opt { get; init; } = new("", "");
            public bool IsAwaitingFirstValue { get; set; } = true;
        }

        private record CommandAndOptionDetails(CommandDef CmdDef, Command Cmd, OptionDetails? currentOption);


    }

    /// <summary>
    /// Class to hold the text being parsed. Provides methods to assist with walking through the text.
    /// </summary>
    internal class TextToParse
    {

        /// <summary>
        /// Text being process as an array of character.s
        /// </summary>
        private char[] chars;

        /// <summary>
        /// The number of characters in our text
        /// </summary>
        private int numChars;

        /// <summary>
        /// The current index point to the position of processing in our character array
        /// </summary>
        private int charIdx;

        public TextToParse(string text)
        {
            this.chars = text.ToCharArray();
            this.numChars = this.chars.Length;
            this.charIdx = 0;

        }

        /// <summary>
        /// Are we are the end of the text being processed?
        /// </summary>
        /// <returns>true if we have reached the end of the text, false if not.</returns>
        public bool isAtEnd()
        {
            if (this.charIdx < this.numChars)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returns the word at the current position in the text being processed and moves forward the current index pointer.
        /// A word is terminated as either reaching a space or end of text.  This method supports quotes.  If the first character
        /// found at the current position is a quote, then all characters (including spaces) are collected up to the closing quote (or end of string).
        /// Quote can be either " or '.  The ending quote must match the starting quotes and be followed by a space or the end of the text.
        /// </summary>
        /// <returns>Details of the Word obtained.</returns>
        public WordDetails GetNextWord()
        {
            StringBuilder word = new(64);

            bool wasQuoted = false;
            bool inQuotes = false;
            char quotedChar = '\0';

            while (this.charIdx < this.numChars)
            {
                char peekChar = this.PeekChar();

                if (peekChar == '"' || peekChar == '\'')
                {
                    if (word.Length == 0 && !inQuotes)
                    {
                        // start of quoted region
                        this.SkipToNextChar();
                        quotedChar = peekChar;
                        inQuotes = wasQuoted = true;
                        continue;
                    }
                    else if (inQuotes && peekChar == quotedChar)
                    {
                        // end of quoted region?
                        char nextChar = this.PeekNextChar();
                        if (nextChar == ' ' || nextChar == '\0')
                        {
                            this.SkipToNextChar();      // skip closing quote
                            this.SkipToNextChar();      // skip space (if not at end)
                            break;
                        }
                    }
                }

                if (!inQuotes && (peekChar == ' ' || peekChar == '='))
                {
                    this.SkipToNextChar();

                    if (word.Length > 0)
                    {
                        break;
                    }
                }
                else
                {
                    word.Append(peekChar);
                    this.SkipToNextChar();
                }
            }

            string s = word.ToString().Trim();
            string unprocessed = (wasQuoted) ? quotedChar + s + quotedChar : s;
            return new WordDetails(s, s.ToUpperInvariant(), wasQuoted, quotedChar, unprocessed);
        }

        private char PeekChar()
        {
            if (this.charIdx < this.numChars)
            {
                return this.chars[charIdx];
            }

            return '\0';
        }


        private void SkipToNextChar()
        {
            if (this.charIdx < this.numChars)
            {
                this.charIdx++;
            }
        }

        private char PeekNextChar()
        {
            int nextCharIdx = this.charIdx + 1;

            if (nextCharIdx < this.numChars)
            {
                return this.chars[nextCharIdx];
            }

            return '\0';
        }
    }

    internal record WordDetails(string Text, string TextUpper, bool WasQuoted, char QuotedChar, string UnProcessed);
    

    internal class ParsingInfo
    {
        public CommandDefCollection commandsFound { get; }

        public ParsingInfo()
        {
            this.commandsFound = new CommandDefCollection();
        }

    }
}

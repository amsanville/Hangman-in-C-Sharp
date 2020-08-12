using System;
using System.IO;
using System.Linq;

namespace Hangman
{
    class Program
    {
        // WORDBANK - the file where the words are stored
        public static readonly string WORDBANK = "wordbank.txt";
        // SCORES - the file where the scores are stored
        public static readonly string SCORES = "scores.txt";
        //
        public static readonly int NUM_SCORES = 5;
        //
        public static readonly int NAME_LENGTH = 6;
        // GRAPHICS - The various text graphics used for the hangman
        public static readonly string[] GRAPHICS = {
            "    ______\n    |    |\n         |\n         |\n         |\n         |\n   ______|",
            "    ______\n    |    |\n    o    |\n         |\n         |\n         |\n   ______|",
            "    ______\n    |    |\n    o    |\n    |    |\n         |\n         |\n   ______|",
            "    ______\n    |    |\n    o    |\n   \\|    |\n         |\n         |\n   ______|",
            "    ______\n    |    |\n    o    |\n   \\|/   |\n         |\n         |\n   ______|",
            "    ______\n    |    |\n    o    |\n   \\|/   |\n    |    |\n         |\n   ______|",
            "    ______\n    |    |\n    o    |\n   \\|/   |\n    |    |\n   /     |\n   ______|",
            "    ______\n    |    |\n    o    |\n   \\|/   |\n    |    |\n   / \\   |\n   ______|"};
        // RENDERFLAGVALS - 
        enum RenderFlag
        {
            NEWWORD,
            ALREADYGUESSED,
            GOODGUESS,
            BADGUESS,
            SOLVED
        }

        /// <summary>
        /// Prints the main menu and processes the user's input.
        /// </summary>
        /// <returns></returns>
        static int MainMenu()
        {
            // Welcome Screen
            Console.Clear();
            Console.WriteLine("Welcome to Hangman");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1.) New Game");
            Console.WriteLine("2.) High Scores");
            Console.WriteLine("3.) Quit");

            // Repeatedly prompt the user for new input
            bool validInput = false;
            string input = "3";
            while (!validInput)
            {
                input = Console.ReadLine().Trim();
                switch (input)
                {
                    case "1":
                        // Write out slection and wait for player before clearing the board
                        Console.WriteLine("New Game Selected. Press Enter to continue.");
                        Console.ReadLine();
                        Console.Clear();
                        validInput = true;
                        break;
                    case "2":
                        // Write out slection and wait for player before clearing the board
                        Console.WriteLine("High Score Selected. Press Enter to continue.");
                        Console.ReadLine();
                        Console.Clear();
                        validInput = true;
                        break;
                    case "3":
                        // Write out slection and wait for player before clearing the board
                        Console.WriteLine("Goodbye!");
                        validInput = true;
                        break;
                    default:
                        Console.WriteLine("Please select a valid option.");
                        break;
                }
            }

            // Return the result
            return Convert.ToInt16(input);
        }

        /// <summary>
        /// Runs the hangman game by repeatedly providing words for the user to guess until they lose. Tallies and saves the score prompting the use to keep playing until they decide to quit.
        /// </summary>
        static void Game()
        {
            // Seed the RNG
            Random rng = new Random();

            // Load in the words
            string[] words = System.IO.File.ReadAllLines(WORDBANK);

            // Keep playing until player decides to quit
            bool quit = false;
            while (!quit)
            {
                // Reset the game
                int currScore = 0;          //<-- The current score of the user
                bool invalidInput = false;  //<-- Flag to validate user input
                int numWrong = 0;           //<-- Number of incorrect guesses made by user
                RenderFlag renderFlag = RenderFlag.NEWWORD; //<-- Flag for what to render as the header
                string currWord = "";       //<-- Current word
                string wordRender = "";     //<-- Version of the word using underscore to fill in for the letters
                string guessedLetters = ""; //<-- List of guessed letters

                // Keep playing until game over
                bool gameOver = false;
                while (!gameOver)
                {
                    // Pick a new word
                    string oldWord = currWord;
                    currWord = words[rng.Next(words.Length)];
                    wordRender = string.Concat(Enumerable.Repeat("_ ", currWord.Length));
                    guessedLetters = "";

                    // Loop for solving each word
                    Console.Clear();
                    bool solved = false;
                    while(!solved && !gameOver)
                    {
                        // Render
                        switch (renderFlag)
                        {
                            case RenderFlag.NEWWORD:
                                Console.WriteLine("New word, make a guess.");
                                break;
                            case RenderFlag.GOODGUESS:
                                Console.WriteLine("Correct!");
                                break;
                            case RenderFlag.BADGUESS:
                                Console.WriteLine("Incorrect!");
                                break;
                            case RenderFlag.SOLVED:
                                Console.WriteLine("Correct! The word was " + oldWord);
                                break;
                            case RenderFlag.ALREADYGUESSED:
                                Console.WriteLine("That letter has already been guessed. Please guess a different letter.");
                                break;
                            default:
                                Console.WriteLine("Congratulations, this is a bug!");
                                break;
                        }
                        Console.WriteLine("Score: " + currScore);
                        Console.WriteLine(GRAPHICS[numWrong]);
                        Console.WriteLine(wordRender);
                        Console.WriteLine("Guessed: " + guessedLetters);

                        // Have player try to guess the letters
                        char guess = 'a';
                        string input = "";
                        invalidInput = true;
                        while (invalidInput)
                        {
                            input = Console.ReadLine().Trim().ToUpper();
                            if (input.Length != 1 || !Char.IsLetter(input[0]))
                            {
                                Console.WriteLine("Please enter a single letter.");
                            }
                            else
                            {
                                invalidInput = false;
                            }
                        }
                        guess = input[0];

                        // Check if the letter has been guessed
                        if (!(guessedLetters.Contains(guess)))
                        {
                            // Check if the letter is in the word
                            if (currWord.Contains(guess))
                            {
                                // Set render flag and update the score
                                renderFlag = RenderFlag.GOODGUESS;
                                currScore++;

                                // Reveal the letters in the word
                                int start = 0;
                                int index = currWord.IndexOf(guess, start);
                                while (index != -1)
                                {
                                    wordRender = wordRender.Substring(0, 2*index) + guess + wordRender.Substring(2*index + 1);
                                    start = index + 1;
                                    index = currWord.IndexOf(guess, start);
                                }
                            }
                            else
                            {
                                // Set render flag and update the score
                                renderFlag = RenderFlag.BADGUESS;
                                numWrong++;
                            }

                            // Add guess to list of guessed letters
                            guessedLetters = guessedLetters + guess + ' ';
                        } else
                        {
                            renderFlag = RenderFlag.ALREADYGUESSED;
                        }

                        // Check if solved or game over
                        if (!wordRender.Contains('_'))
                        {
                            solved = true;
                            currScore += 5;
                            renderFlag = RenderFlag.SOLVED;
                        }
                        if (numWrong == GRAPHICS.Length - 1)
                        {
                            gameOver = true;
                        }


                        // Clear to restart the loop
                        Console.Clear();
                    }
                }

                // Final render
                Console.WriteLine("Game Over! The word was: " + currWord);
                Console.WriteLine("Score: " + currScore);
                Console.WriteLine(GRAPHICS[numWrong]);
                Console.WriteLine(wordRender);
                Console.WriteLine("Guessed: " + guessedLetters);

                // Load the scores
                string[] loadedScores = new string[NUM_SCORES];
                using (StreamReader sr = new StreamReader(SCORES))
                {
                    // Read in scores using stream reader
                    for (int i = 0; i < NUM_SCORES; i++)
                    {
                        loadedScores[i] = sr.ReadLine();
                    }
                }

                // Parse the scores
                string[] names = new string[NUM_SCORES];
                int[] scoreValues = new int[NUM_SCORES];
                for(int i = 0; i <  NUM_SCORES; i++)
                {
                    string[] splitScores = loadedScores[i].Split(' ');
                    names[i] = splitScores[0]; 
                    scoreValues[i] = Convert.ToInt16(splitScores[1]);
                }

                // Check if score is bigger than any of the high scores
                bool found = false;
                int scoreIndex = 0;
                for(; scoreIndex < NUM_SCORES && !found; scoreIndex++)
                {
                    if (scoreValues[scoreIndex] <= currScore)
                    {
                        found = true;
                    }
                }

                // Update the scores
                if (found)
                {
                    string newName = "STEVE!";
                    Console.WriteLine("New high score! Enter your name:");
                    bool unconfirmed = true;
                    while (unconfirmed)
                    {
                        // Read in the name and enforce name length condition
                        newName = Console.ReadLine().Trim().ToUpper();
                        if (newName.Length > NAME_LENGTH)
                        {
                            newName = newName.Substring(0, NAME_LENGTH);
                        }
                        else if (newName.Length < NAME_LENGTH)
                        {
                            newName = newName.PadRight(NAME_LENGTH);
                        }

                        // Confirm the name with the user
                        Console.WriteLine("Is " + newName + " correct? Y/N");
                        invalidInput = true;
                        while (invalidInput)
                        {
                            string input = Console.ReadLine().Trim().ToLower();
                            switch (input)
                            {
                                case "y":
                                case "yes":
                                    invalidInput = false;
                                    unconfirmed = false;
                                    break;
                                case "n":
                                case "no":
                                    Console.WriteLine("Enter your name: ");
                                    invalidInput = false;
                                    break;
                                default:
                                    Console.WriteLine("Please enter yes or no.");
                                    break;
                            }
                        }
                    }

                    // Move the scores and names
                    for (int i = NUM_SCORES - 1; i >= scoreIndex; i--)
                    {
                        names[i] = names[i - 1];
                        scoreValues[i] = scoreValues[i - 1];
                    }

                    // Enter in the new scores and names
                    names[scoreIndex - 1] = newName;
                    scoreValues[scoreIndex - 1] = currScore;
                    for(int i = 0; i < NUM_SCORES; i++)
                    {
                        loadedScores[i] = names[i] + " " + scoreValues[i];
                    }

                    // Overwrite the existing scores file
                    // TODO: REDO WITH STREAMS
                    using (StreamWriter sw = new StreamWriter(SCORES))
                    {
                        for (int i = 0; i < NUM_SCORES; i++)
                        {
                            sw.WriteLine(loadedScores[i]);
                        }
                    }
                }

                // See if user wants to play again
                Console.WriteLine("Play again? Y/N");
                invalidInput = true;
                while (invalidInput)
                {
                    string input = Console.ReadLine().Trim().ToLower();
                    switch (input)
                    {
                        case "y":
                        case "yes":
                            invalidInput = false;
                            break;
                        case "n":
                        case "no":
                            invalidInput = false;
                            quit = true;
                            break;
                        default:
                            Console.WriteLine("Please enter yes or no.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Provides a run down 
        /// </summary>
        static void Scores()
        {
            // Load the scores
            using(StreamReader sr = new StreamReader(SCORES))
            {
                // Read in scores using stream reader
                string[] loadedScores = new string[NUM_SCORES];
                for(int i = 0; i < NUM_SCORES; i++)
                {
                    loadedScores[i] = sr.ReadLine();
                }

                // Print out the scores
                Console.Clear();
                for (int i = 0; i < loadedScores.Length; i++)
                {
                    Console.WriteLine(loadedScores[i]);
                }
            }

            
            Console.WriteLine("Press enter to return to the main menu.");
            Console.ReadLine();
        }

        /// <summary>
        /// Runs the game by first starting the main menu and quitting when
        /// appropriate.
        /// </summary>
        /// <param name="args">Unused.</param>
        static void Main(string[] args)
        {
            // Make a scores file
            if (!File.Exists(SCORES))
            {
                using (StreamWriter sw = new StreamWriter(SCORES))
                {
                    for (int i = 0; i < NUM_SCORES; i++)
                    {
                        sw.WriteLine(new string(((char)(((int)'A') + i)), NAME_LENGTH) + " " + (NUM_SCORES - i));
                    }
                }
            }
            // Check if there is a wordbank
            if (File.Exists(WORDBANK))
            {
                // Start the game loop
                bool quit = false;
                while (!quit)
                {
                    // Choose what's next based on what happens on the menu
                    switch (MainMenu())
                    {
                        case 1:
                            // 1 Player mode
                            Game();
                            break;
                        case 2:
                            // 2 Player Mode
                            Scores();
                            break;
                        case 3:
                            // Quit
                            quit = true;
                            break;
                        default:
                            // *Should* be impossible to reach
                            Console.WriteLine("Congratulations, there's a bug and you stumbled into it!");
                            break;

                    }
                }
            } else
            {
                Console.WriteLine("Error: no word bank to use for hangman.");
            }
        }
    }
}

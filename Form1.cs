using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MisteryNumber_12_oct_2017 {
    public partial class frmMainDSte : Form {

        private Boolean showCheatsButtonsDSte = false;
        private int generatedNumberDSte = 0;
        private int previousGuessDSte = 0;
        private int minValueDSte = 0;
        private int maxValueDSte = 0;
        private int guessesLeftDSte = 10;
        private Random randomDSte = new Random();

        public frmMainDSte() {
            //Initalise all the components
            InitializeComponent();
            initDSte();

            //Set up a listener for the hotkeys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(FrmMainDSte_KeyDown);

        }

        #region init and reset code
        //This initialises the default values
        private void initDSte() {
            debug("setting default values");
            this.txbGuessDSte.Text = "";
            this.lblGuessesLeft.Text = "";
            this.lblMisteryNumber.Text = "0000";
            this.lblDiffDSte.Text = "0000";
            this.lblHotColdDSte.Text = "";
            this.lblPrevGuessDSte.Text = "";
            this.txbMinDSte.Text = "1";
            this.txbMaxDSte.Text = "100";
            this.comBxAttemptsDSte.Text = "10";
            this.comBxAttemptsDSte.Items.Clear();
            this.comBxAttemptsDSte.Items.Add("5");
            this.comBxAttemptsDSte.Items.Add("10");
            this.comBxAttemptsDSte.Items.Add("20");
            this.comBxAttemptsDSte.Items.Add("30");
            this.comBxAttemptsDSte.Items.Add("40");
            this.comBxAttemptsDSte.Items.Add("80");
            this.comBxAttemptsDSte.Items.Add("100");
            this.showCheatsButtonsDSte = false;
            this.checkCheatDSte.Visible = showCheatsButtonsDSte;
            this.checkExtraDSte.Visible = showCheatsButtonsDSte;
            this.comBxAttemptsDSte.Enabled = true;
            this.btnGenAndStartDSte.Enabled = true;
            this.txbMaxDSte.Enabled = true;
            this.txbMinDSte.Enabled = true;
            this.checkCheatDSte.Checked = false;
            this.checkExtraDSte.Checked = false;
            this.btnGuessDSte.Enabled = false;
            this.progBarGuessesLeftDSte.Value = 0;
            this.minValueDSte = 0;
            this.maxValueDSte = 0;
            this.previousGuessDSte = 0;
            this.generatedNumberDSte = 0;
            this.previousGuessDSte = 0;
            this.trackDistDSte.SetRange(0, 1);
            this.txbMaxDSte.Select(0, 0);
            this.Height = 250; //Max is 335
            this.Width = 310; //Max is 433
            logFormItemValues();
            debug("done");
        }
        #endregion

        #region cheat and extra
        private void checkCheatDSte_CheckedChanged(object sender, EventArgs e) {
            Boolean isCheckedDSte = checkCheatDSte.Checked;
            debug("Checkbox for cheats changed", isCheckedDSte);
            if (isCheckedDSte) {
                this.Height = 335; //Max is 335
            } else {
                this.Height = 250; //Max is 335
            }
        }

        private void CheckExtraDSte_CheckedChanged(object sender, EventArgs e) {
            Boolean isCheckedDSte = checkExtraDSte.Checked;
            debug("Checkbox for extra changed", isCheckedDSte);
            if (isCheckedDSte) {
                this.Width = 433; //Max is 433
            } else {
                this.Width = 310; //Max is 433
            }
        }

        private void FrmMainDSte_KeyDown(object sender, KeyEventArgs e) {
            //Hey you cheater, I see what you are doing.
            //Trying to get the key code for enabeling the cheats (It's not ctrl-F10)
            if (e.Control && e.KeyCode == Keys.F10) {
                debug("Cheats and extras toggled");
                this.showCheatsButtonsDSte = !showCheatsButtonsDSte;
                this.checkCheatDSte.Visible = showCheatsButtonsDSte;
                this.checkExtraDSte.Visible = showCheatsButtonsDSte;
                //This gets rid of that annoying windows sound
                e.SuppressKeyPress = true;
            }
        }

        private void BtnManual_Click(object sender, EventArgs e) {
            //Read the text from the help text file
            String helpTextDSte = File.ReadAllText(Application.StartupPath + "\\..\\..\\assets\\manual\\manual.txt");
            //Display the text in a notepad window
            NotepadHelperDSte.ShowMessage(helpTextDSte, "Mistery Number Manual");
            //But instead we show a boring pdf file
            //Process.Start("explorer.exe", Application.StartupPath + "\\..\\..\\assets\\manual\\manual.pdf");
        }

        private void BtnLocateDSte_Click(object sender, EventArgs e) {
            //This opens an explorer window with the app in it
            Process.Start("explorer.exe", Application.StartupPath);
        }

        private void BtnAboutDSte_Click(object sender, EventArgs e) {
            //Dialog with the creator
            MessageBox.Show("Mistery number\n\n" +
                "Created by: Duncan \"duncte123\" Sterken", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnExitDSte_Click(object sender, EventArgs e) {
            //We ask if the user wants to quit
            DialogResult resultDSte = MessageBox.Show("Do you realy want to quit the game?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (resultDSte == DialogResult.Yes)  {
                //Yes
                Application.Exit();
            }
        }

        private void btnRestartDSte_Click(object sender, EventArgs e) {
            //We ask if the user wants to restart
            DialogResult resultDSte = MessageBox.Show("Do you realy want to restart the game?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (resultDSte == DialogResult.Yes) {
                //Yes
                this.initDSte();
            }
        }

        #endregion

        #region Main game functions
        private void btnGenAndStartDSte_Click(object sender, EventArgs e) {
            //Get the range
            this.minValueDSte = getIntFromTextDSte(txbMinDSte.Text);
            this.maxValueDSte = getIntFromTextDSte(txbMaxDSte.Text);
            //Make sure that the max isn't lower then the min
            if (this.maxValueDSte < this.minValueDSte) {
                MessageBox.Show("Max value can't be heiger than the min value", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Create the random number
            this.generatedNumberDSte = randomDSte.Next(minValueDSte, maxValueDSte);
            this.lblMisteryNumber.Text = this.generatedNumberDSte + "";
            //Set the min and max for the trackbar
            this.trackDistDSte.Maximum = this.maxValueDSte;
            this.trackDistDSte.Value = this.maxValueDSte;
            this.guessesLeftDSte = getIntFromTextDSte(this.comBxAttemptsDSte.Text);
            //Disable the settings to prevent the user from generating a new number
            this.comBxAttemptsDSte.Enabled = false;
            this.btnGenAndStartDSte.Enabled = false;
            this.txbMaxDSte.Enabled = false;
            this.txbMinDSte.Enabled = false;
            this.btnGuessDSte.Enabled = true;
            this.progBarGuessesLeftDSte.Maximum = this.guessesLeftDSte;
            this.progBarGuessesLeftDSte.Value = this.guessesLeftDSte;
            this.lblGuessesLeft.Text = this.guessesLeftDSte + "";
            debug("---------------------------------------------------------------------------");
            debug("minValueDSte", this.minValueDSte);
            debug("maxValueDSte", this.maxValueDSte);
            debug("comBxAttemptsDSte.Enabled", this.comBxAttemptsDSte.Enabled);
            debug("btnGenAndStartDSte.Enabled", this.btnGenAndStartDSte.Enabled);
            debug("txbMaxDSte.Enabled", this.txbMaxDSte.Enabled);
            debug("txbMinDSte.Enabled", this.txbMinDSte.Enabled);
            debug("btnGuessDSte.Enabled", this.btnGuessDSte.Enabled);
            debug("progBarGuessesLeftDSte.Maximum", this.progBarGuessesLeftDSte.Maximum);
            debug("comBxAttemptsDSte.Enabled", this.comBxAttemptsDSte.Enabled);
            debug("generatedNumberDSte", this.generatedNumberDSte);
            debug("guessesLeftDSte", this.guessesLeftDSte);
            debug("---------------------------------------------------------------------------");
        }

        private void txbGuessDSte_KeyDown(object sender, KeyEventArgs e) {
            //If we hit enter in the guess box we will to the same as pressing the guess button
            if (e.KeyCode == Keys.Enter && this.btnGuessDSte.Enabled) {
                btnGuessDSte_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void btnGuessDSte_Click(object sender, EventArgs e) {
            //Get the guessed number
            int guessedNumberDSTe = getIntFromTextDSte(txbGuessDSte.Text);
            debug("Guessed number", guessedNumberDSTe);
            debug("Prev guess", this.previousGuessDSte);
            //Check if it is in the range
            if (guessedNumberDSTe > this.maxValueDSte || guessedNumberDSTe < this.minValueDSte) {
                debug("Guess was out of range");
                MessageBox.Show("Your guess is not between " +
                    this.minValueDSte + " and " + this.maxValueDSte + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //Subtract 1 from the guesses left
            this.guessesLeftDSte--;
            this.lblGuessesLeft.Text = guessesLeftDSte + "";
            debug("Guesses left", this.guessesLeftDSte);
            //Checks if we have guessed the number
            if (guessedNumberDSTe.Equals(generatedNumberDSte)) {
                debug("Player won game");
                playSoundDSTe("win_sound.wav");
                DialogResult resultDSte = MessageBox.Show("You won.\n\n" +
                    "Play again?", "Congrats", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (resultDSte == DialogResult.Yes) {
                    //Yes
                    this.initDSte();
                } else {
                    //No
                    Application.Exit();
                }
                return;
                //Checks if we have no guesses left
            } else if (this.guessesLeftDSte <= 0) {
                debug("Player lost game");
                playSoundDSTe("loose_sound.wav");
                DialogResult resultDSte = MessageBox.Show("You are out of guesses.\n" +
                    "The mistery number was " + this.generatedNumberDSte + "\n\n" +
                    "Play again?", "RIP", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (resultDSte == DialogResult.Yes) {
                    //Yes
                    this.initDSte();
                } else {
                    //No
                    Application.Exit();
                }
                return;
            }
            //Get the difference to the random number
            int diffDSte = this.generatedNumberDSte - guessedNumberDSTe;
            this.progBarGuessesLeftDSte.Value--;
            this.lblPrevGuessDSte.Text = this.previousGuessDSte + "";
            debug("diff raw", diffDSte);
            String diffStringDSte = ("" + diffDSte).Replace("-", "");
            String hotColdStringDSte = "" + ((this.previousGuessDSte < guessedNumberDSTe) ? "Warmer" : "Colder");
            
            if (Math.Abs(this.generatedNumberDSte - guessedNumberDSTe) < Math.Abs(this.generatedNumberDSte - this.previousGuessDSte)) {
                hotColdStringDSte = "Warmer";
            } else {
                hotColdStringDSte = "Colder";
            }

            this.lblHotColdDSte.Text = hotColdStringDSte;
            this.lblDiffDSte.Text = diffStringDSte;
            this.trackDistDSte.Value = getIntFromTextDSte(diffStringDSte);
            //The last thing we do is setting the last guess to the current guess
            this.previousGuessDSte = guessedNumberDSTe;
        }

        /// <summary>
        /// Converts a string to an int
        /// </summary>
        /// <param name="toConvert">The string to convert</param>
        /// <returns>The number that you enterd, if it isn't a number it will return 1</returns>
        private int getIntFromTextDSte(String toConvert) {
            try {
                //Use a regex to strip all the non numerical chars
                return Convert.ToInt32(new Regex("[^0-9]").Replace(toConvert, ""));
            }
            catch {
                return 1;
            }
        }

        /// <summary>
        /// This plays an audio file wthe the given name from assets/sound
        /// </summary>
        /// <param name="fileName">The file name to play</param>
        private void playSoundDSTe(String fileName) {
            //Stop the player before playing it so we know that we can play a file
            StopAudio();
            PlayAudio(fileName);
        }

        //debug logging
        private void debug(String message, Object obj) {
            Console.WriteLine("[DEBUG] " + message + ": " + (obj != null ? obj.ToString() : "null"));
        }
        private void debug(String message) {
            Console.WriteLine("[DEBUG] " + message);
        }
        private void logFormItemValues() {
            foreach(Control c in this.Controls) {
                Type type = c.GetType();
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo property in properties) {
                   debug(c.Name + "." + property.Name , property.GetValue(c, null));
                }
                //
            }
        }

        #endregion

        #region Audio handling code
        //Start default audio handling code
        //None of this is my code, all credit for this code goes to Jbra
        //dll import to add audio
        [DllImport("winmm.dll")]

        private static extern long mciSendString(
        string strCommand,
        StringBuilder strReturn,
        int iReturnLength,
        IntPtr hwndCallback);

        private void PlayAudio(String fileName) {
            //play audio
            mciSendString("open \"" +
                          Application.StartupPath +
                          "\\..\\..\\assets\\sound\\"
                          + fileName
                          + "\" type mpegvideo alias MediaFile", null, 0, IntPtr.Zero);

            mciSendString("play MediaFile", null, 0, IntPtr.Zero);
        }

        private static void StopAudio() {
            //Stop audio if audio already played
            mciSendString("close MediaFile", null, 0, IntPtr.Zero);
        }

        //End default audio handling code
        #endregion
    }
}

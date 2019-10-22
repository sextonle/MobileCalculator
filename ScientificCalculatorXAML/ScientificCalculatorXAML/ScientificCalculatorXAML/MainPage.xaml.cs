using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScientificCalculatorXAML
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]

    /*
     * Please run in UWP
     * The calculator is centered in the middle of the UWP screen
     * in approx phone size. This is due to the UWP  window not going small
     * enough of a phone size
     * 
     * As many error cases as I could find on the online version have been handled 
     * (though some could have been missed) Some examples include:
     * Hitting a operator first and doing the operation with a zero
     * Hitting 2 operators in a row replaces the original
     * Display starts over after = or memory clicked
     * Waits for a second value to be entered if you have entered in say 5+=
     * User can toggle between making the first value negative and positive
     * 
     * Assumptions:
     * only first number needs to be a negative
     */

    public partial class MainPage : ContentPage
    {
        Queue<double> values;
        Queue<String> operators;
        double memory;
        String tempInt;
        String currentBtnType;

        public MainPage()
        {
            InitializeComponent();
            roundValChanged(rounderPicker);
            values = new Queue<double>();
            operators = new Queue<string>();
            memory = 0;
            tempInt = "";
            currentBtnType = "";
        }

        //generating all the options for the user to select rounding values past decimal
        void roundValChanged(object sender)
        {
            Picker p = (Picker)sender;
            for (int i = 0; i < 15; i++)
            {
                p.Items.Add(i.ToString());
            }
            p.SelectedIndex = 3;
        }//end round val changed method

        //what to display when a digit is clicked
        void digitClicked(object sender, EventArgs e)
        {
            Button digitBtn = (Button)sender;
            //gets rid of leading zero
            if(display.Text.Equals("0"))
            {
                display.Text = "";
            }

            //after hitting equal or memory the display should start over
            if(currentBtnType.Equals("equals") | currentBtnType.Equals("xFactorial") | currentBtnType.Equals("mem"))
            {
                display.Text = "";
                values.Clear();
            }
            currentBtnType = "digit";
            switch (digitBtn.Text)
            {
                case "0":
                    tempInt += "0";
                    display.Text += "0";
                    break;

                case "1":
                    tempInt += "1";
                    display.Text += "1";
                    break;

                case "2":
                    tempInt += "2";
                    display.Text += "2";
                    break;

                case "3":
                    tempInt += "3";
                    display.Text += "3";
                    break;

                case "4":
                    tempInt += "4";
                    display.Text += "4";
                    break;

                case "5":
                    tempInt += "5";
                    display.Text += "5";
                    break;

                case "6":
                    tempInt += "6";
                    display.Text += "6";
                    break;

                case "7":
                    tempInt += "7";
                    display.Text += "7";
                    break;

                case "8":
                    tempInt += "8";
                    display.Text += "8";
                    break;

                case "9":
                    tempInt += "9";
                    display.Text += "9";
                    break;

                case ".":
                    tempInt += ".";
                    display.Text += ".";
                    break;

                case "π":
                    double piRnd = Math.Round(Math.PI, rounderPicker.SelectedIndex);
                    tempInt = piRnd.ToString();
                    display.Text = piRnd.ToString();
                    addTempInt();
                    break;

                //allows the user to toggle between negative and positive value
                case "±":
                    if (display.Text.Contains("-"))
                    {
                        display.Text = display.Text.Remove(0, 1);
                        tempInt = tempInt.Remove(0, 1);
                    }
                    else
                    {
                        display.Text = display.Text.Insert(0, "-");
                        tempInt = tempInt.Insert(0, "-");
                    }
                    break;
            }
        }//end digit clicked

        //what should be displayed if delete or clear buttons are clicked
        void DelExpClearClicked(object sender, EventArgs e)
        {
            Button Btn = (Button)sender;
            //currentBtnType = "delClear";
            switch (Btn.Text)
            {
                case "DEL": 
                    if ((display.Text.Length > 1) && (display.Text != "0"))
                    {
                        if(currentBtnType.Equals("operator"))
                        {
                            operators.Dequeue();
                            display.Text = display.Text.Substring(0, (display.Text.Length) - 1);
                            tempInt = values.Dequeue().ToString(); 
                        }
                        else 
                        {
                            display.Text = display.Text.Substring(0, (display.Text.Length) - 1);
                            if(tempInt.Length > 0)
                            {
                                tempInt = tempInt.Substring(0, (tempInt.Length) - 1);
                            } 
                        }
                    }
                    else
                    {
                        display.Text = "0";
                        values.Clear();
                        operators.Clear();
                    }
                    break;

                case "C":
                    display.Text = "0";
                    values.Clear();
                    operators.Clear();
                    tempInt = "";
                    break;
            }
            currentBtnType = "delClear";
        }//end delete and clear method

        //what is displayed if the equals button is clicked
        void equalsClicked(object sender, EventArgs e)
        {
            Button equalBtn = (Button)sender;
            currentBtnType = "equals";
            addTempInt();
            double result = 0;

            //if there is more than one operator display -1
            if (operators.Count > 1)
            {
                values.Clear();
                operators.Clear();
                display.Text = "-1";
            }
            //if there are no operators then clear display
            else if(operators.Count == 0)
            {
                currentBtnType = "equals";
            }
            //hitting equals with 1 or 0 values waits for you to enter in a value
            else if(values.Count <= 1)
            {
                currentBtnType = "digit";
            }
            //normal case
            else
            {
                if(values.Count > 1)
                {
                    switch (operators.Dequeue())
                    {
                        case "+":
                            result = (values.Dequeue() + values.Dequeue());
                            appendResult(result);
                            break;

                        case "-":
                            result = (values.Dequeue() - values.Dequeue());
                            appendResult(result);
                            break;

                        case "x":
                            result = (values.Dequeue() * values.Dequeue());
                            appendResult(result);
                            break;

                        case "÷":
                            result = (values.Dequeue() / values.Dequeue());
                            appendResult(result);
                            break;

                        case "e":
                            result = (values.Dequeue() * (Math.Pow(10, values.Dequeue())));
                            /*tempInt += result;
                            display.Text = result.ToString();*/
                            appendResult(result);
                            break;

                        case "^":
                            result = Math.Pow(values.Dequeue(), values.Dequeue());
                            appendResult(result);
                            break;
                            /*case "±":
                                display.Text = "NaN";
                                break;*/
                    }
                }                
            }
        }//end equals clicked method

        //if the momory buttons are clicked
        void memClicked(object sender, EventArgs e)
        {
            Button memBtn = (Button)sender;
            currentBtnType = "mem";
            switch (memBtn.Text)
            {
                case "M+":
                    addTempInt();
                    memory = memory + values.Peek();
                    memoryDisplay();
                    break;

                case "M-":
                    addTempInt();
                    memory = memory - values.Peek();
                    memoryDisplay();
                    break;

                case "MR":
                    display.Text = memory.ToString();
                    break;

                case "MS":
                    addTempInt();
                    memory = values.Peek();
                    break;

                case "MC":
                    memory = 0;
                    display.Text = memory.ToString();
                    break;
            }
        }

        //if an operator is clicked
        void operatorClicked(object sender, EventArgs e)
        {
            Button opBtn = (Button)sender;
            addTempInt();
            if (values.Count == 0)
            {
                tempInt = "0";
                addTempInt();
            }
            if(currentBtnType.Equals("operator"))
            {
                operators.Dequeue();
                display.Text = display.Text.Substring(0, (display.Text.Length) - 1);
            }
            currentBtnType = "operator";
            switch (opBtn.Text)
            {
                case "+":
                    operators.Enqueue("+");
                    display.Text += "+";
                    break;

                case "-":
                    operators.Enqueue("-");
                    display.Text += "-";
                    break;

                case "x":
                    operators.Enqueue("x");
                    display.Text += "x";
                    break;

                case "÷":
                    operators.Enqueue("÷");
                    display.Text += "÷";
                    break;

                case "exp":
                    operators.Enqueue("e");
                    display.Text += "e";
                    break;
                case "x^y":
                    operators.Enqueue("^");
                    display.Text += "^";
                    break;
                
            }
        }//end operator clicked method

        //how to handle if a yellow button is clicked
        void miniOperationClicked(object sender, EventArgs e)
        {
            Button miniOpBtn = (Button)sender;
            currentBtnType = "miniOperation";
            addTempInt();
            double result = 0;
            double value = 0;
            if(values.Count != 0)
            {
                value = values.Dequeue();
            }
            switch (miniOpBtn.Text)
            {
                case "sin":
                    result = Math.Sin(value);
                    appendResult(result);
                    break;
                case "asin":
                    result = Math.Asin(value);
                    appendResult(result);
                    break;
                case "cos":
                    result = Math.Cos(value);
                    appendResult(result);
                    break;
                case "acos":
                    result = Math.Acos(value);
                    appendResult(result);
                    break;
                case "tan":
                    result = Math.Tan(value);
                    appendResult(result);
                    break;
                case "atan":
                    result = Math.Atan(value);
                    appendResult(result);
                    break;

                case "√":
                    result = Math.Sqrt(value);
                    appendResult(result);
                    break;
                case "x^2":
                    result = Math.Pow(value, 2);
                    appendResult(result);
                    break;
                case "10^x":
                    result = Math.Pow(10, value);
                    appendResult(result);
                    break;
                case "ln":
                    result = Math.Log(value);
                    appendResult(result);
                    break;
                case "log":
                    result = Math.Log10(value);
                    appendResult(result);
                    break;
                case "1/x":
                    result = (1/value);
                    appendResult(result);
                    break;
                case "x!":
                    result = xfactorial(value);
                    appendResult(result);
                    currentBtnType = "xFactorial";
                    break;
            }
        }//end mini operation method

        //adding the temporary integer to the values queue
        void addTempInt()
        {
            if (tempInt.Length > 0)
            {
                double tempVal = Double.Parse(tempInt, System.Globalization.NumberStyles.AllowDecimalPoint |
                 System.Globalization.NumberStyles.AllowLeadingSign);

                    values.Enqueue(tempVal);
                    tempInt = "";
            }
        }//end add temp int method

        //once a result is computed round it and display
        void appendResult(double result)
        {
            result = Math.Round(result, rounderPicker.SelectedIndex);
            tempInt = result.ToString();
            //values.Enqueue(result);
            display.Text = result.ToString();
        }//end append result

        //computing x factorial
        int xfactorial(double x)
        {
            int fact = (int)x;
            for (int i = (int)x - 1; i >= 1; i--)
            {
                fact = fact * i;
            }
            return fact;
        }//end factorial method

        //display the memory calculations in the window once computed
        void memoryDisplay()
        {
            display.Text = memory.ToString();
            values.Clear();
            values.Enqueue(memory);
        }//end memory display
    }
}

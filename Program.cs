using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;


namespace QuizAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            EdgeDriver edgeDriver = new EdgeDriver();

            IJavaScriptExecutor javascriptExecutor = (IJavaScriptExecutor)edgeDriver;
            javascriptExecutor.ExecuteScript("window.open('http://bilinch.net/hans/home.php', '_blank', 'toolbar=yes,scrollbars=yes,resizable=yes,width=device-width,height=device-height')");
            
            try
            {
                edgeDriver.SwitchTo().Window(edgeDriver.WindowHandles[1]);                
                
                edgeDriver.Url = "http://bilinch.net/hans/";

                edgeDriver.FindElementByXPath("//*[@id=\"username\"]").SendKeys("admin");
                edgeDriver.FindElementByXPath("//*[@id=\"password\"]").SendKeys("admin123" + Keys.Enter);

                edgeDriver.SwitchTo().Window(edgeDriver.WindowHandles[0]);

                for (int i = 2; i <= 26; i++)
                {
                    edgeDriver.Url = "https://milyonist.com/tv/milyoner/soru/listele/maddi/15bin";
                    
                    string address = edgeDriver.FindElementByXPath($"/html/body/div[1]/article/div[1]/div/div[{i}]/div[2]/a").GetAttribute("href");
                    edgeDriver.Url = address;

                    try
                    {
                        edgeDriver.FindElementByXPath("//*[@id=\"Layer_1\"]").Text.Replace("\u00AD", "");
                        Console.WriteLine("There is voice!");
                    }
                    catch
                    {
                        string question = edgeDriver.FindElementByXPath("/html/body/div[1]/article/div[1]/div[1]/section[1]/div[2]/p").Text.Replace("\u00AD", "");
                        
                        Console.WriteLine($"QUESTION: {question}");

                        IWebElement webElement;
                        string rightChoice = "";

                        for (int i2 = 1; i2 <= 2; i2++)
                        {
                            for (int i3 = 1; i3 <= 2; i3++)
                            {
                                webElement = edgeDriver.FindElementByXPath($"/html/body/div[1]/article/div[1]/div[1]/section[1]/div[3]/div[{i2}]/div[{i3}]/div");

                                string choice = webElement.FindElement(By.CssSelector(".Multiple__letter")).Text;
                                string answer = webElement.FindElement(By.CssSelector(".Multiple__text")).Text;

                                Console.WriteLine($"{choice}: {answer}");

                                if (webElement.GetAttribute("class").Contains("Multiple__right_answer"))
                                {
                                    rightChoice = choice;
                                    // TODO: select right choice
                                }
                            }
                        }

                        Console.WriteLine($"RIGHT CHOICE: {rightChoice}");
                    }
                }
            }
            finally
            {
                edgeDriver.Quit();
            }
        }
    }
}

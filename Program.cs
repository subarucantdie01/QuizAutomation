using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;

using QuizAutomation.Utils.Data;

namespace QuizAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            EdgeDriver edgeDriver = new EdgeDriver();

            IJavaScriptExecutor javascriptExecutor = edgeDriver;
            javascriptExecutor.ExecuteScript("window.open('', '_blank', 'toolbar=yes,scrollbars=yes,resizable=yes,width=device-width,height=device-height')");

            edgeDriver.SwitchTo().Window(edgeDriver.WindowHandles[1]);

            edgeDriver.Url = "http://bilinch.net/hans";

            edgeDriver.FindElementByXPath("//*[@id=\"username\"]").SendKeys("admin");
            edgeDriver.FindElementByXPath("//*[@id=\"password\"]").SendKeys("admin123" + Keys.Enter);

            Thread.Sleep(100);

            edgeDriver.Url = "http://bilinch.net/hans/questions.php";

            PropertiesFile dataProperties = Properties.GetPropertiesFileLocally("data.properties");

            int page = int.Parse(dataProperties.GetValue("page"));

            while (true)
            {
                DateTime now = DateTime.Now;

                for (int i = int.Parse(dataProperties.GetValue("index")); i <= 26; i++)
                {
                    edgeDriver.SwitchTo().Window(edgeDriver.WindowHandles[0]);

                    edgeDriver.Url = $"https://milyonist.com/tv/milyoner/soru/listele/maddi/60bin?sayfa={page}";
                    edgeDriver.Url = edgeDriver.FindElementByXPath($"/html/body/div[1]/article/div[1]/div/div[{i}]/div[2]/a").GetAttribute("href");

                    try
                    {
                        edgeDriver.FindElementById("sesli");
                        Console.WriteLine("Voice found, skipped!");
                    }
                    catch (NoSuchElementException)
                    {
                        string question = edgeDriver.FindElementByClassName("Question__text").Text.Replace("\u00AD", "");
                        Console.WriteLine($"QUESTION: {question}");

                        IReadOnlyCollection<IWebElement> webElements = edgeDriver.FindElementsByClassName("Multiple__choices");

                        List<string> choices = new List<string>();
                        string rightChoice = "";
                        
                        foreach (IWebElement webElement in webElements)
                        {
                            string choice = webElement.FindElement(By.ClassName("Multiple__letter")).Text;
                            string answer = webElement.FindElement(By.ClassName("Multiple__text")).Text;

                            choices.Add(answer);

                            Console.WriteLine($"{choice}: {answer}");

                            if (webElement.GetAttribute("class").Contains("Multiple__right_answer"))
                                rightChoice = choice;
                        }

                        Console.WriteLine($"RIGHT CHOICE: {rightChoice}");

                        edgeDriver.SwitchTo().Window(edgeDriver.WindowHandles[1]);

                        new SelectElement(edgeDriver.FindElementByXPath("//*[@id=\"category\"]")).SelectByValue("9");

                        edgeDriver.FindElementByXPath("//*[@id=\"question\"]").SendKeys(question);

                        for (int cI = 0; cI < choices.Count; cI++)
                        {
                            string choice = cI == 0 ? "a" : (cI == 1 ? "b" : (cI == 2 ? "c" : (cI == 3 ? "d" : "")));
                            edgeDriver.FindElementByXPath($"//*[@id=\"{choice}\"]").SendKeys(choices[cI]);
                        }

                        edgeDriver.FindElementByXPath("//*[@id=\"level\"]").SendKeys("3");

                        new SelectElement(edgeDriver.FindElementByXPath("//*[@id=\"answer\"]")).SelectByValue(rightChoice.ToLower());

                        edgeDriver.FindElementByXPath("//*[@id=\"submit_btn\"]").Click();
                    }

                    dataProperties.SetAndSave("index", (i != 26 ? i + 1 : 2) + "");
                }

                dataProperties.SetAndSave("page", ++page + "");

                TimeSpan elapsed = DateTime.Now - now;

                Console.WriteLine($"{elapsed.Minutes} minutes/{elapsed.Seconds} seconds passed!");
            }
        }
    }
}

const assert = require('assert');
const { Given, When, Then, setWorldConstructor } = require('cucumber');
const {Builder, By, Key, until} = require('selenium-webdriver'); 
const chrome = require('selenium-webdriver/chrome');
const firefox = require('selenium-webdriver/firefox');
const jsotp = require('jsotp');
const util = require('util')
const fs = require('fs')
const writeFile = util.promisify(fs.writeFile)
 
  setWorldConstructor(function (init) { 
    this.parameters = init.parameters;
    if (this.driver == null)
    {  
      this.driver = new Builder()
                        .forBrowser('chrome')
                        .usingServer('http://localhost:4444/wd/hub')
                        .build();
    }
    this.clearLocalStorageItem = function(driver) {
      return driver.executeScript("localStorage.clear()");
    } 
  });

  Given('I\'m on the mealplanner website', async function () { 
    console.log("Website tested: " + this.parameters.mpUrl);
    await this.driver.get(this.parameters.mpUrl);
  });

  Given('That I\'m not logged in', function () { 
    this.clearLocalStorageItem(this.driver);
  });

  When('I log in with {string} and no token', async function (name) {
    await this.driver.get(this.parameters.mpUrl);
    var inputName = await this.driver.findElement(By.id('name'));
    inputName.sendKeys(name); 
    var loginBtn = await this.driver.findElement(By.className('btn btn-primary'));
    loginBtn.sendKeys(Key.RETURN); 
  });
  
  When('I log in with {string} and bad token', async function (name) {
    await this.driver.get(this.parameters.mpUrl);
    var inputName = await this.driver.findElement(By.id('name'));
    inputName.sendKeys(name);
    var inputPassword = await this.driver.findElement(By.id('token'));
    inputPassword.sendKeys('notCorrect');
    var loginBtn = await this.driver.findElement(By.className('btn btn-primary'));
    loginBtn.sendKeys(Key.RETURN); 
  });

  When('I log in with {string} and correct generated token', async function (name) {
    await this.driver.get(this.parameters.mpUrl);
    var inputName = await this.driver.findElement(By.id('name'));
    inputName.sendKeys(name);
  
    var totp = jsotp.TOTP('MHZZKBHWO2VCHZYHPD56IDIUTSJJ5FVI');
    var inputPassword = await this.driver.findElement(By.id('token'));
    inputPassword.sendKeys(totp.now());
      
    var loginBtn = await this.driver.findElement(By.className('btn btn-primary'));
    loginBtn.sendKeys(Key.RETURN); 
    
  });

  When('I look at the site', function () {
    
  });

  Then('A error message should shown {string}', async function (expected) {
    await this.driver.wait(until.elementLocated(By.className("alert-danger")), 3000);
    var element = await this.driver.findElement(By.className('alert-danger'));
    assert.equal(await element.getText(), expected);
    
    let image = await this.driver.takeScreenshot();
    await writeFile('./output_error_'+expected+'.png', image, 'base64'); 
    await this.driver.quit();
  });

   Then('I will see the dashboard with {string}', async function (expected) {
    await this.driver.wait(until.elementLocated(By.tagName("i")), 3000);
    var element = await this.driver.findElement(By.tagName('i'));
    assert.equal(await element.getText(), expected);
    let image = await this.driver.takeScreenshot();
    await writeFile('./output_dashboard.png', image, 'base64'); 
    await this.driver.quit();
  });

  Then('I will see in section {string} the name {string}', async function (credits, expected) {
    try
    {
      await this.driver.wait(until.elementLocated(By.className("col-sm-8")), 3000);
      var element = await this.driver.findElement(By.xpath('//*[@id="app"]/div/div/div/div[2]/div[1]'));
      var innerText = await element.getText(); 
      assert.equal(innerText.indexOf(expected) > -1, true);
      let image = await this.driver.takeScreenshot();
      await writeFile('./output_homepage.png', image, 'base64'); 
    }
    catch(e)
    {
      console.log(e)
    }
    await this.driver.quit();
  });


   
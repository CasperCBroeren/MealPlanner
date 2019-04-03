const assert = require('assert');
const { Given, When, Then } = require('cucumber');
const {Builder, By, Key, until} = require('selenium-webdriver'); 
const chrome = require('selenium-webdriver/chrome');
const firefox = require('selenium-webdriver/firefox');
const jsotp = require('jsotp');



  Given('I\'m on the mealplanner website', async function () {
    var opts = new firefox.Options();     
  //  opts.headless(true);
    this.driver = new Builder()
	.forBrowser('firefox')
	.usingServer('http://localhost:4444/wd/hub')
    //.setFirefoxOptions(opts)
    .build();
    await this.driver.get('http://maaltijdplanner.azurewebsites.net');
  });

  Given('That I\'m not logged in', function () { 
    clearLocalStorageItem(this.driver);
  });

  When('I log in with {string} and no token', async function (name) {
    await this.driver.get('http://maaltijdplanner.azurewebsites.net');
    var inputName = await this.driver.findElement(By.id('name'));
    inputName.sendKeys(name); 
    var loginBtn = await this.driver.findElement(By.className('btn btn-primary'));
    loginBtn.sendKeys(Key.RETURN); 
  });
  
  When('I log in with {string} and bad token', async function (name) {
    await this.driver.get('http://maaltijdplanner.azurewebsites.net');
    var inputName = await this.driver.findElement(By.id('name'));
    inputName.sendKeys(name);
    var inputPassword = await this.driver.findElement(By.id('token'));
    inputPassword.sendKeys('notCorrect');
    var loginBtn = await this.driver.findElement(By.className('btn btn-primary'));
    loginBtn.sendKeys(Key.RETURN); 
  });

  When('I log in with {string} and correct generated token', async function (name) {
    await this.driver.get('http://maaltijdplanner.azurewebsites.net');
    var inputName = await this.driver.findElement(By.id('name'));
    inputName.sendKeys(name);
  
    var totp = jsotp.TOTP('MHZZKBHWO2VCHZYHPD56IDIUTSJJ5FVI');
    var inputPassword = await this.driver.findElement(By.id('token'));
    inputPassword.sendKeys(totp.now());
      
    var loginBtn = await this.driver.findElement(By.className('btn btn-primary'));
    loginBtn.sendKeys(Key.RETURN); 
    
  });

  Then('A error message should shown {string}', async function (expected) {
    await this.driver.wait(until.elementLocated(By.className("alert-danger")), 3000);
    var element = await this.driver.findElement(By.className('alert-danger'));
    assert.equal(await element.getText(), expected);
    await this.driver.quit();
  });

  Then('I will see the dashboard with {string}', async function (expected) {
    await this.driver.wait(until.elementLocated(By.tagName("i")), 3000);
    var element = await this.driver.findElement(By.tagName('i'));
    assert.equal(await element.getText(), expected);
    await this.driver.quit();
  });

  function clearLocalStorageItem(driver) {
    return driver.executeScript("localStorage.clear()");
  }

   
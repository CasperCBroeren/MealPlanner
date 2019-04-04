Feature: MealplannerLogin 
    Login in to mealplanner 

    Scenario: Login incorrect no token
        Given I'm on the mealplanner website
        Given That I'm not logged in
        When I log in with "CJIB" and no token
        Then  A error message should shown "Vul ook de token van je authenticator in"
        
    Scenario: Login incorrect token
        Given I'm on the mealplanner website
        Given That I'm not logged in
        When I log in with "CJIB" and bad token
        Then  A error message should shown "Je token is niet geledig"

    Scenario: Login correct token
        Given I'm on the mealplanner website
        Given That I'm not logged in
        When I log in with "CJIB" and correct generated token
        Then  I will see the dashboard with 'CJIB'

    Scenario: Credits should contain Joyce
        Given I'm on the mealplanner website
        Given That I'm not logged in
        When I look at the site
        Then  I will see in section 'Credits' the name 'Joyce'
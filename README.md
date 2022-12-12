# ASSETS MANAGEMENT.
* **

#### This is my pet project. 
This project helps me with managing all my assets.
It can be cash, shares of public companies, crypto assets, real estate and etc.

##### It started in April 2018 and was changed many times in several repositories.
I have finished the project versions, but I don't like their design. I'm improving it's here.
Also, here I'm testing new technologies, new approaches, new patterns, and new designs.

This development is huge. It always improves. And here are many points that I must finish.

It isn't finished, so I don't describe all the configurations and public parameters for working with it.
But completed services you can run with the docker-compose file. You also need to run the infrastructure docker-compose file from the '/src/am_infrastructure'. And you need to configure .env for both 'compose' files.

### The functionality of main services.
#
* ##### Portfolio (CURRENT DEVELOPING)
    * This service receives and keeps transactions on my assets.
    * It can parse transaction reports from my providers of reports about transactions.
    * I can get data about my assets transactions with the REST API.
    * It can calculate any summary data for me.
    * It sends some data to the recommendations service for building recommendations for me.
* ##### Analyzer (DEPRECATED. TOWARD REWRITING)
    * This service collects public data from any sources about public companies, exchange rates of currency and cryptocurrency, and the costs of real estate.
    * It processes this collected data, computes coefficients, and builds asset ratings from this.
    * I can get information about collected data and ratings with the REST API.
    * It sends rating data to the recommendations service for recommendations for me.
* ##### Recommendations (DEPRECATED. TOWARD REWRITING)
    * This service receives data from other services and computes it. Then it builds recommendations for me. 
    * For example, I can get information about that, which shares of public companies I need to buy or sell.
    * I can get this information with the REST API.
    * And also, I have the Telegram bot notifications if some prices achieve the required values.
* ##### Web client (DEPRECATED. TOWARD REWRITING)
    * This client creates a SPA app with the React tech.
    * I have a web client as PWA with the Blazor tech in another repository.

### If you need to start services but they don't work. Just notify me.
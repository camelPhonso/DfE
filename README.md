## What is this?

This codebase mocks a small distributed solution for providing a data pipeline from the public API from the Department for Education.
This is a learning project and the documentation included is for future notes - when I don't remember anymore how to do any of this 
work.

Jason, if you're lurking on my github again, hope you enjoy this one. All the best dude <3.

The project assumes a series of limitations to keep scope under control and because constraint and creativity something something...
    - the purpose of this system is to regularly check for new publications from the DfE and pull the latest when available
    - only one version is required at any point - the latest publication
    - once the data has been retrieved, ingestion and surfacing can be handled elsewhere; this is not about manipulating the data
    - in this fantasy realm, we must work with Microsoft products, por lo que sea.

Given these, the following plan is drawn out:
    - Azure Function App triggered periodically (new publications are expected each semester, so every ~3 months).
        - Metadata available from the DfE API allows us to track the current version for each publication against
          our stored data.
    - Once pulled through, data can be dumped into Blob storage just as is.

### External resources

[Department for Education statistics API](https://api.education.gov.uk/statistics/docs/getting-started/)

## Local environment

The codebase includes a docker-compose file that will provide the following containerized services:

    - Wiremock mapping two endpoints from the DfE API
    - Azurite local storage emulator to simulate blob storage

This allows integration testing and a more complete experience while developing locally. The Azure Function App can be developed locally and tested fairly comprehensively without needing to pay Bill Gates to host some
infrastructure just so I can keep my wits sharp.

Run these:
    
```shell
    docker compose up -d
    # This will start up the local environment so that
    # calls to the API will be mocked and requests to
    # storage will query the local emulator instead.
```

```shell
    dotnet run
    # The codebase can be run as a dotnet application
    # instead of using 'func start' via the extension
    # for Azure Functions development.
```

## A duck

🦆
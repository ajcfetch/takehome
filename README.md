# Receipt Processor - Dockerized .NET Application

Welcome to the **Fetch Receipt Processor** application! This project is a .NET application, uses ConcurrentDictionary to manage session data, and is fully containerized with Docker for easy deployment.

---

## Quick Start

### **Prerequisites**
Before running the application, ensure you have the following installed:
- 🐳 **[Docker](https://docs.docker.com/get-docker/)** (must be running)

### **Clone the Repository**
```sh
git clone https://github.com/ajcfetch/takehome.git
cd takehome/FetchReceiptProcessor
```

### **Build the Docker Image**
```sh
docker build -t receipt-processor .
```

### **Run the Container**
```sh
docker run -d -p 5074:5074 --name receipt-container receipt-processor
```
---

## **Access the Application**
### **Open in Browser or Postman**
Once the container is running, access the application at:
```
http://localhost:5074
```
---

### **Run Automated Tests**
```sh
cd takehome
cd docker build -t fetch-receipt-tests -f Dockerfile.tests .
cd docker rm -f receipt-tests
```

---

## Decision Considerations
  
I tried to leave notes in the form of comments with `// Note:` where I had opinions or general thoughts. 

My overall thought process in creating this is as follows:
- Start with getting git repos + making sure I had the latest versions of VSCode + dotnet ready to go
- I wanted to start with setting up the Models, that is a way to help me get acclimated with the requirements without being overwhelmed.
  - Immediately I wanted to make sure they were using proper data types, so I created some reusable json converters.
  - I left a note, but I went back and forth on if I wanted to keep date and time separate or lump together, idk if it really matters either way. 
  - I used decimal for price because I know if you don't you can get really odd rounding issues
  - I then spun up some quick unit tests for these json converters  
- Next I moved on to making some dummy controller methods to just make sure I could access them fine in postman and get valid responses. 
- I then moved on to mapping out my service layer, but decided I wanted to actually go down a level and start with the data layer. 
  - ConcurrentDictionary is what I chose to use for the in memory data because it was quick and this app requirement didn't have intense querying. Otherwise I'd have chosen litedb or something. 
  - I tried to treat this as a "repo" layer still so it handles just the adding and retrieving of receipts, no logic 
  - Once that was good, made some unit tests
- Moved on to service layer
  - I started with the two public functions for saving a receipt and getting points for an id. 
  - I knew I needed to do some extra validation for adding a receipt, so I made some generic validators. I'm not super sold on my solution for this, and I absolutely wouldn't not be using strings to represent the errors in a production app - I would choose keys or something that can be easily translatable, but didn't want to overcomplicate. Also I made a lot of assumptions, tried to note the them where I did. 
  - I then made the calculate points, I also think there is a ton of room here to extract this out and make it way more usable, but again, didn't want to overcomplicate without more context. 
  - I made the calculate and validate functions be internal so they could still be unit tested, because there is a lot of logic in those. 
  - I made unit tests for those as I went
  - I also added in some custom exceptions here, I don't think it's always necessary but I've found it does make for easier debugging and nicer for organization of exceptions for front end handling when logic and requests get complex
- I went back to the controller and called the new services, caught the exceptions, and tested it all out. 
  - I also made integration tests for this. 
- Next I got docker all setup, made this readme, and am going to publish to github while I await your thoughts. 
- I did not add swagger, probably should/would, but I am writing this now as I wrap up this take home and I'd like to go play with my kid :) 
- Sorry I'm back, one final note before I upload - I really value structure in a backend and adhere to keeping things in the layers they belong. It may feel like overkill at first when you don't "need" it but I've been burned when I didn't do it and stuff grew and grew and it became so much harder to refactor and add new features. This takes much less time to do and to keep clean long term. 

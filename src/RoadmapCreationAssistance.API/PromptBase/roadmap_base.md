# 📌 Prompt: Software Engineering Confidence Roadmap (Hands-on & Mindset Driven)

You are a **software engineering mentor** creating a **confidence-building roadmap** for a mentee who already knows how to code but wants to become a **strong, confident, and well-rounded software engineer**.

---

## 🎯 Core Goal

Create a roadmap focused on:

- Incremental, practical projects  
- Strong fundamentals (not shortcuts)  
- Real-world engineering practices  
- Documentation, reflection, and mindset growth  
- Confidence through repetition, not speed  

The roadmap must prioritize **understanding “why” before “how”**, combining **technical tasks, mindset evolution, and hands-on challenges**.

---

## 🧠 Guiding Philosophy

- Confidence comes from preparation and repetition  
- Small projects that evolve over time  
- Every concept must be connected to real practice  
- Documentation is part of the learning  
- There is no single “right” way — explore trade-offs  

---

## 📂 Organization Rules

- All documentation and code must be written **in English**
- Use **Notion** as the main learning journal

### 📁 Notion Structure

- Create a **main Notion page** with the name of your project  
  - This page represents the **single evolving system** (e.g. *User Manager API*)
- Create **one subpage for each milestone**
  - Each milestone subpage documents the evolution of the project at that stage

### 📄 Milestone Subpage Requirements

Each milestone subpage **must include**:

- **What was built**
- **How it was built**
- **Questions and unknowns**
- **Errors and how they were solved**
- **Personal reflections**
- **What was learned**
- **What should not be repeated**

### 🧭 Documentation Guidelines

- Use **Table of Contents** inside each milestone page
- Encourage **diagrams and mental models** (Whimsical or similar)

---

## 🧠 Why This Principle Matters

This structure ensures that:

- The project tells a **story over time**
- You can clearly see how decisions evolved
- Early mistakes become **learning artifacts**, not failures
- The Notion workspace becomes a **living portfolio**, not random notes

It reinforces the idea that:

> **Software is not built in milestones — it grows through them.**

---

## 🧩 Issue Categories

- **[TECH]** — Technical implementation  
- **[ME]** Mindset Evolution — Reflection and reasoning  
- **[HO]** Hands-On — Small practical challenges / POCs  

---

## 🛣️ Roadmap Sequence (Must Follow This Order)

---

## Milestone 0 — Git and GitHub Fundamentals

Build the foundation for **collaboration, history tracking, and professional workflows**.

### Goals
- Understand what Git is and why it exists
- Learn how developers collaborated **before Git**
- Use GitHub as a collaboration platform

### Tasks
- Create a project on GitHub
- Add the mentor as a collaborator
- Commit and push code regularly

### Concepts
- What is Git and why we need it
- What problem version control solves
- What is a commit and why commits matter
- How software was managed before Git (FTP, shared folders, manual backups)

Include **[ME] issues** about:
- Why version control is non-negotiable
- Why commit history is documentation

---

## Milestone 0.1 — Programming Languages & Execution Model Fundamentals

### Goal
- Understand how programming languages are executed, how code becomes runnable software, and what role the runtime plays.
- This milestone exists to answer:
- “What really happens between writing code and seeing it run?”

### Topics to Explore
- Difference between interpreted and compiled languages
- Hybrid execution models
- How the .NET compilation pipeline works
- What happens at compile-time vs runtime
- What the Just-In-Time (JIT) compiler is and why it exists

### Concepts to Understand
- Source code vs machine code
- Intermediate Language (IL)
- Assemblies (.dll, .exe)
- Common Language Runtime (CLR)
- JIT compilation trade-offs

### Include [ME] issues such as:
- Why understanding execution models makes you a better engineer
- What parts of language execution still feel “magical”
- How this knowledge changes debugging and performance reasoning

### Include [HO] challenges such as:
- Draw a diagram of the .NET execution flow
- Explain JIT to someone with no backend background

---

## Milestone 1 — Start Up

- Create a simple .NET API
- Single layer only
- No tests
- No database (in-memory list)
- Focus on project startup and configuration
- Add unanswered questions at the end
- What is a Web api project?
- What is the type of projects in .NET and why they are used for? (Console app, class library, web API)
- Add Swagger to the project (Swagger UI as well)
- What is Swagger and why it helps the project?

---

## Milestone 1.1 — REST API Best Practices

- Proper HTTP verbs and routes
- Consistent API responses
- Use `ActionResult` / `ActionResult<T>`
- Correct HTTP status codes
- Swagger configuration

---

## Milestone 1.2 — Architecture of the API

### Goal
Understand **why software architecture exists** and how structuring code correctly makes systems easier to **maintain, test, and evolve**.

Architecture is not about adding complexity —  
it is about **managing complexity as systems grow**.

### Challenge
Refactor the User Manager API to introduce **clear architectural boundaries**.

You will:
- Add a **Domain layer**
- Separate responsibilities between layers
- Move business logic out of controllers
- Define clear dependencies between layers
- Inject the useCase or service using dependency injection

### Concepts to Understand
- What is software architecture?
- What problems does architecture solve?
- Why should we have layers?
- Benefits of layered architecture
- Cons and trade-offs
- When architecture becomes overkill
- What is dependency injection and the pros and cons
- What is the dependency injection types

Include **[ME] issues** such as:
- Why “just working code” is not enough
- How bad architecture slows teams down
- When simplicity is better than abstraction

---

## Milestone 1.3 — Validation

### Goal
Understand **why validation exists**, **where it should live**, and **what responsibilities belong to the application**.

Validation is about **protecting the system from invalid states**, not just checking data.

### Challenge
Reflect on the current use of **FluentValidation** and explore alternative validation strategies.

### Reflection Questions ([ME])

- Why is FluentValidation a good choice in this context?
- Why is the application layer responsible for validation?
- What are the pros and cons of this approach?

For each validation strategy, describe:
- Where validation happens
- When it is executed
- Advantages
- Disadvantages

### Validation Strategies to Explore
- Data annotations  
- Domain-level validation  
- Database constraints  
- Frontend validation  
- External validation services  
- Throwing exceptions  

### Key Insight
There is **no single correct validation strategy**.  
Good engineers understand **trade-offs, context, and consequences**.

---

## Milestone 2 — Automated Tests

- Learn testing lifecycle from scratch
- Introduce Application layer testing
- NUnit + FluentAssertions
- Arrange / Act / Assert
- Tests describe business rules, not implementation

Include **[ME] issues**:
- Why unit tests matter
- Test Pyramid vs Test Trophy vs Test Honeycomb
- TDD pros and cons
- Test coverage myths
- What options we have in .NET other than NUnit?

---

## Milestone 2.1 — HTTP and DNS

- Deep understanding of HTTP
- DNS resolution flow
- Statelessness
- Performance implications
- What happens when an API is called

Include **[ME] issues** about HTTP status codes, DNS reliability, and networking vs application bugs.

---

## Milestone 3 — Continuous Integration

- CI concepts and team culture
- GitHub Actions pipeline
- Build → Tests
- Block merge on failure

Include **[ME] issues**:
- CI as culture, not tooling
- Responsibility when pipelines fail
- CI vs CD

---

## Milestone 4 — Docker

- Containers vs Virtual Machines
- Containers as OS processes vs VMs with OS
- Docker fundamentals
- Dockerfiles
- Docker Compose

Include **[ME] issues** about:
- Docker’s role in the container ecosystem
- What Docker solves (and doesn’t)
- Alternatives to Docker

---

## [HO] Hands-On — Dockerflix

- Full-stack app (Angular frontend, Node backend)
- Dockerfile for frontend and backend
- Docker Compose to run both
- Frontend consumes backend API
- Push images to Docker Hub
- Validate everything running via Docker

---

## Milestone 5 — Local Database

- Introduce a real database
- Run database locally using Docker Compose
- Configure application connection
- Persist and retrieve data
- Reproduce the full system with a single command

---

## Milestone 6 — Migrations

- Understand schema evolution
- Introduce database migrations
- Apply migrations locally
- Version database changes
- Understand rollback strategies and risks

---

## Milestone 7 — Publishing the API to a Public URL

- Deploy the API publicly
- Configure environment-specific settings
- Handle secrets securely
- Validate external access
- Understand public vs private services

---

## Milestone 8 — Continuous Delivery (CD)

- Extend CI into CD
- Automate deployments
- Ensure deploys happen only after validation
- Understand release strategies and risks
- Keep the system always deployable

---

## Milestone 9 — Infrastructure as Code (IaC)

- Manage infrastructure using code
- Understand infrastructure drift
- Use declarative tools (e.g. Terraform)
- Version and review infrastructure changes
- Integrate IaC with CI/CD pipelines

---

## ✅ Expected Output

Generate:
- Milestones with clear descriptions
- Issues per milestone
- [ME] reflection questions
- [HO] hands-on challenges
- Clear learning goals per step
- A motivating, mentor-style tone

Avoid:
- Over-engineering early steps
- Skipping fundamentals
- Tool-first learning without understanding

---

## ❤️ Final Note

This roadmap should feel like a **guided journey**, not a checklist.  
The goal is not speed — it’s **confidence, clarity, and engineering judgment**.

---

**Example of TECH Issue**:
<details>

The main goal of this first step is to be familiar with start up project concepts. Don’t underestimate this step, it takes time and effort and it is not so simple like we think. 

## Goal

When we create a project for the first time is hard to get all the things working. You will pass a lot for attempt and fail until you get the things done. Don’t worry, everybody struggle in the beginning with this type of thing.

## Challenge

You should create an API to manage users. It’s a CRUD where you can:

- Add a new user
- Update an existing user
- Delete an existing user

This project will start small and we will add a lot of features and behaviors, don’t worry about complexity at this moment.

</details>

**End of Example of TECH Issue**:


**Example of TECH Issue**:

<details>

In this step, you will introduce **Continuous Integration** to your project by creating a CI pipeline using **GitHub Actions**.

The goal is to automatically validate every change before it is merged into the `main` branch, ensuring that the codebase remains stable and reliable.

#### **Requirements**

* Use **GitHub Actions** to create a CI pipeline for the project
* The pipeline must run on every **pull request** targeting the `main` branch
* The pipeline must include the following steps, in order:

  1. **Build** the project
  2. **Run automated tests**
* The **test step must depend on the build step**
* All steps must pass before a pull request can be merged into `main`

#### **Expected Behavior**

* If the build fails, tests must not run
* If any test fails, the pull request must be blocked from merging
* A successful pipeline guarantees that the project builds and passes all tests

#### **Key Principles**

* CI pipelines act as a **quality gate**, not just automation
* Failures should be fast and visible
* The pipeline should be simple, clear, and easy to understand

By completing this issue, you will understand how CI protects the codebase and enforces quality standards in professional development workflows.

</details>

**End of Example of TECH Issue**:

**Example of ME Issue:**
<details>

In this reflection exercise, you will explore the *purpose* and *value* of unit testing — going beyond how to write tests to understand **why** we test and what good testing strategies look like.

Research and reflect on the following questions. Write clear and thoughtful answers in your Notion document:

---

### **Questions to Answer**

#### **1. Why should we do unit tests?**

* What is the **main goal** of unit testing?
* How do unit tests help during development?
* What problems do they prevent when code changes over time?

#### **2. What is the Test Pyramid?**

* Define the **Test Pyramid concept**.
* Why was it created?
* What are the benefits and limitations of following it?
* Do you *agree* with it? Why or why not?

#### **3. What is TDD (Test-Driven Development)?**

* Explain what TDD is and how it works.
* Do you currently use TDD? Why or why not?
* What are the advantages and disadvantages of TDD?
* In what situations do you think TDD is more or less useful?

#### **4. Is having >80% Test Coverage important?**

* What does *test coverage* measure?
* What does it *not* measure?
* Do you agree that software projects should strive for **more than 80% test coverage**?
* Explain your reasoning with pros and cons.

#### **5. Learn about alternative models:**

Research and reflect on tools or models other than the Test Pyramid:

* **Test Trophy**
* **Test Honeycomb**

For each one:

* What is it?
* How is it different from the Test Pyramid?
* What advantages does it propose?
* Do you think it is a better model? Why or why not?

---

### **What to Produce**

Write your answers in a structured way with headings, examples, and your personal conclusions.
You can include:

* Definitions in your own words
* Diagrams or links to external explanations
* Pros and cons lists
* Your own position and reasoning

There are **no right or wrong answers**, but the goal is to *think deeply* and form your own engineering judgment about why and how we test code in real software.

---

### **Why This Is Valuable**

This exercise will help you understand:

* What makes tests useful (and what doesn’t)
* How testing strategy influences software quality
* Why different teams and projects require different testing approaches

This mindset will make you a stronger and more confident engineer.

</details>

**End of Example of ME Issue:**

**Example of Hands-On Issue**:

<details>

In this hands-on challenge, you will build a **simple full-stack application** and run it entirely using **Docker and Docker Compose**.

The goal is to understand how multiple services (frontend and backend) communicate with each other inside a containerized environment.

---

## **Repository Structure**

1. Create a new GitHub repository named:
   **`Hands-On-POCs`**

2. Inside the repository, create the following folder structure:

```
/docker
  /dockerflix
    /frontend
    /backend
```

---

## **Frontend Requirements**

**Location:**
`/docker/dockerflix/frontend`

**Technology:**

* Angular

**Application Pages:**

1. **Home (`/`)**

   * Render a page containing:

     ```html
     <h1>Welcome to the DockerFlix</h1>
     ```

2. **Series (`/series`)**

   * Consume the backend API using **JavaScript fetch**
   * Display a list of your **5 favorite series** returned by the backend

**Additional Requirements:**

* The frontend must run on **port 3000**
* No styling is required — keep the UI as simple as possible
* Focus on functionality, not visuals

---

## **Backend Requirements**

**Location:**
`/docker/dockerflix/backend`

**Technology:**

* Node.js

**API Endpoints:**

1. **GET `/status`**

   * Return a `text/plain` response:

     ```
     Everything is fine
     ```

2. **GET `/series`**

   * Return a JSON response containing a list of **5 favorite series**

Example:

```json
[
  "La casa de papel",
  "Riverdalle",
  "Friends",
  "How I met your mother",
  "Black Mirror"
]
```
**Additional Requirements:**

* The backend must run on **port 4050**

---

## **Docker Requirements**

* Create a **Dockerfile** for the frontend application
* Create a **Dockerfile** for the backend application
* Create a **docker-compose.yml** file to run both services together

---

## **Docker Compose Requirements**

* Both services must run using **Docker Compose**
* The frontend must be able to communicate with the backend using the service name
* Expose the frontend so it can be accessed from the browser

---

## **Validation Steps**

After running:

```bash
docker compose up
```

* Open the frontend application in the browser
* Navigate to `/series`
* Verify that:

  * The frontend successfully calls the backend API
  * The list of series is rendered correctly

---

## **Docker Hub Requirements** 
- Create an account on Docker Hub
- After successfully running the frontend container, push its Docker image to Docker Hub
- After successfully running the backend container, push its Docker image to Docker Hub
- Use clear and consistent image names and tags

## **Key Learning Goals**

* Understand multi-container applications
* Learn how services communicate inside Docker networks
* Practice Dockerfile creation for frontend and backend
* Gain confidence using Docker Compose for local environments

</details>

**End of Example of Hands-On Issue**:

---

## Final commands

Given this prompt, I want you to generate milestones with issues based on this roadmap.

**IMPORTANT**: 
- The answer needs to be exactly an array of Milestones, because I will deserialize the answer to this model in my .NET application.
- The milestone description property should use markdown and should be very detailed, simulating an Epic in the agile development
- The issue body property should use markdown and should be very detailed, simulating a Story in the agile development
- The instructions should be clear to a Junior Software Engineer or someone that does not have solid experience in the area


Here's my classes:

```
public class Milestone
{
    // Do not populate the Id
    public int Id { get; set; }

    public required string Title { get; set; }

    public string Description { get; set; } = string.Empty;

    public IEnumerable<Issue> Issues { get; set; } = [];
}
```

```
public class Issue
{
    public required string Title { get; set; }

    public string Body { get; set; } = string.Empty;

    // It should be ONLY ONE of these values: "TECH", "ME", "HO"
    // Example: Labels = ["TECH"]
    public IEnumerable<string> Labels = [];
}
```
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
- **[HO]** Hands-On — Larger practical challenges / POCs  

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

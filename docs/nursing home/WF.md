# Workflow for IT Department

## ERP System Development for Nursing Home Operations

## 1. Project Objective

The objective of the ERP system is to create a centralized operating platform for nursing home branches. The system should help management monitor patients, staff, care activities, admissions, billing, inventory, expenses, branch performance, and compliance in real time.

The ERP should support both single-branch operations and future multi-branch expansion.

---

# 2. Core System Goals

The ERP system should help the nursing home group achieve the following:

1. Standardize branch operations.
2. Improve patient care documentation.
3. Reduce manual paperwork.
4. Improve communication between caregivers, nurses, branch managers, head office, and families.
5. Track revenue, costs, occupancy, and profitability by branch.
6. Support quality control and audit.
7. Improve staff scheduling and accountability.
8. Prepare the business for scale, franchise, investors, and bank reporting.

---

# 3. Key User Groups

The IT team should design the ERP based on the needs of different users.

| User Group                      | Main Usage                                                                          |
| ------------------------------- | ----------------------------------------------------------------------------------- |
| Head Office Management          | View dashboard, financials, branch KPIs, occupancy, quality reports                 |
| Branch Manager                  | Manage daily operations, staff, admissions, expenses, branch performance            |
| Nurse Manager                   | Manage care plans, health records, medication, incidents, care quality              |
| Nursing Assistants / Caregivers | Record daily care activities, meals, bathing, toileting, mobility, sleep, incidents |
| Sales / Admission Team          | Manage leads, inquiries, visits, quotations, admissions, follow-ups                 |
| Accounting / Finance            | Manage billing, payments, revenue, expenses, AR, AP, branch P&L                     |
| HR / Admin                      | Manage staff records, shifts, attendance, leave, training                           |
| Inventory / Procurement         | Manage medical consumables, food, equipment, stock usage                            |
| Family / Relatives Portal       | View updates, invoices, care summaries, announcements, visit schedules              |
| System Admin                    | Manage user rights, branch setup, permissions, master data                          |

---

# 4. ERP Development Phases

## Phase 1: Business Requirement Study

### Objective

Understand the actual nursing home operation before developing the system.

### IT Department Tasks

1. Interview head office management.
2. Interview branch managers.
3. Interview nurse managers.
4. Interview caregivers.
5. Interview sales and admission team.
6. Interview accounting team.
7. Review all current forms and reports.
8. Observe actual branch operations.
9. Identify manual processes.
10. Identify pain points.
11. Identify duplicated work.
12. Identify critical reporting needs.

### Key Deliverables

* Business Requirement Document
* Current Workflow Map
* Pain Point Analysis
* User Role Matrix
* Required Report List
* Initial System Scope

---

## Phase 2: Process Mapping

### Objective

Convert nursing home operations into clear system workflows.

### Core Workflows to Map

| Workflow               | Description                                                       |
| ---------------------- | ----------------------------------------------------------------- |
| Lead to Admission      | Inquiry, family consultation, site visit, quotation, admission    |
| Patient Onboarding     | Document collection, health assessment, contract, room assignment |
| Daily Care Record      | Meals, bathing, toileting, medication, mobility, sleep, mood      |
| Nursing Assessment     | Vital signs, symptoms, care plan, risk level, follow-up           |
| Medication Management  | Medication list, schedule, administration record, missed dose     |
| Incident Reporting     | Fall, injury, abnormal symptom, complaint, emergency transfer     |
| Staff Scheduling       | Shift planning, attendance, leave, OT, replacement staff          |
| Billing and Collection | Monthly invoice, payment status, deposit, outstanding balance     |
| Expense Management     | Branch expenses, approval, supporting documents, petty cash       |
| Inventory Management   | Stock request, stock usage, low-stock alert, procurement          |
| Family Communication   | Daily / weekly updates, announcements, visit schedule             |
| Quality Audit          | Branch inspection, checklist, score, corrective action            |
| Management Reporting   | Occupancy, revenue, EBITDA, patient mix, staff ratio, incidents   |

### Key Deliverables

* Future Workflow Design
* Approval Flow Design
* Data Flow Diagram
* Branch Operation Blueprint
* System Module Map

---

## Phase 3: ERP Module Design

### Objective

Define the ERP modules required for nursing home operations.

### Recommended ERP Modules

| Module                             | Purpose                                                            |
| ---------------------------------- | ------------------------------------------------------------------ |
| 1. Master Data Management          | Branch, room, bed, patient type, care package, price list          |
| 2. CRM / Lead Management           | Inquiry, lead source, family contact, follow-up, site visit        |
| 3. Admission Management            | Patient onboarding, contract, documents, initial assessment        |
| 4. Patient Profile                 | Personal data, medical history, emergency contact, care level      |
| 5. Care Plan Management            | Individual care plan, activities, risk level, special instructions |
| 6. Daily Care Record               | Daily tasks, meals, bathing, toileting, sleep, mobility, mood      |
| 7. Nursing Record                  | Vital signs, symptoms, wound care, medication, clinical notes      |
| 8. Medication Management           | Medication schedule, administration record, alerts                 |
| 9. Incident Management             | Falls, injuries, complaints, emergency, corrective action          |
| 10. Staff Scheduling               | Shift roster, attendance, leave, OT, staff ratio                   |
| 11. HR Management                  | Staff records, training, certificates, performance                 |
| 12. Billing and Payment            | Invoice, receipt, deposit, payment tracking, overdue               |
| 13. Accounting Interface           | Revenue, expenses, branch P&L, export to accounting software       |
| 14. Procurement and Inventory      | Consumables, food, equipment, stock level, purchase request        |
| 15. Family Portal                  | Updates, invoice, care summary, visit booking                      |
| 16. Branch Dashboard               | Occupancy, revenue, staff, incidents, collections                  |
| 17. Head Office Dashboard          | Multi-branch performance, KPI, financial summary                   |
| 18. Quality Audit                  | Inspection checklist, audit score, action plan                     |
| 19. Document Management            | Contracts, consent forms, ID cards, medical documents              |
| 20. User and Permission Management | Access rights by role, branch, and function                        |

---

# 5. Recommended MVP Scope

The first version should focus on essential operations. Do not build everything at once.

## MVP Modules

| Priority | Module                      | Reason                                     |
| -------- | --------------------------- | ------------------------------------------ |
| High     | Patient Profile             | Core database for all operations           |
| High     | Admission Management        | Controls new patient onboarding            |
| High     | Daily Care Record           | Most important nursing home activity       |
| High     | Nursing Record              | Required for care quality and risk control |
| High     | Medication Management       | High-risk operational area                 |
| High     | Billing and Payment         | Directly linked to cash flow               |
| High     | Staff Scheduling            | Critical for operations                    |
| High     | Branch Dashboard            | Needed by branch manager                   |
| Medium   | CRM / Lead Management       | Important for sales growth                 |
| Medium   | Inventory Management        | Helps control cost                         |
| Medium   | Incident Management         | Important for quality and legal protection |
| Medium   | Head Office Dashboard       | Needed for multi-branch expansion          |
| Later    | Family Portal               | Valuable but can be Phase 2                |
| Later    | Full HR Management          | Can be integrated later                    |
| Later    | Full Accounting Integration | Can start with export first                |

---

# 6. Detailed Workflow by Function

## 6.1 Lead to Admission Workflow

### Step 1: Lead Entry

Sales or admin enters:

* Lead name
* Family contact person
* Phone number
* Line ID
* Lead source
* Patient age
* Care condition
* Budget range
* Preferred location
* Urgency level

### Step 2: Consultation

System records:

* Family needs
* Patient condition
* Required care level
* Preferred room type
* Budget expectation
* Follow-up date

### Step 3: Site Visit

System records:

* Visit date
* Branch visited
* Salesperson
* Family feedback
* Probability of admission

### Step 4: Quotation

System generates:

* Care package
* Monthly fee
* Deposit
* Additional charges
* Validity period

### Step 5: Admission Confirmation

System changes status from Lead to Patient.

---

## 6.2 Patient Onboarding Workflow

### St
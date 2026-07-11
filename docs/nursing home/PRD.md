ERP Product Requirement Document



Nursing Home Operations ERP System



Version 1.0



⸻



1\. Project Overview



ระบบ ERP สำหรับธุรกิจ Nursing Home นี้มีเป้าหมายเพื่อเป็นระบบกลางในการบริหารงานสาขา ดูแลผู้พัก ควบคุมต้นทุน จัดการบุคลากร ออกบิล ติดตามรายได้ ตรวจสอบคุณภาพบริการ และรายงานผลการดำเนินงานให้สำนักงานใหญ่ นักลงทุน และธนาคาร



ระบบต้องสามารถรองรับการใช้งานตั้งแต่ 1 สาขา ไปจนถึงหลายสาขา เช่น 40 สาขา และขยายต่อไปถึง 100 สาขาในอนาคต



⸻



2\. Business Objectives



ระบบ ERP นี้ต้องช่วยให้ธุรกิจ Nursing Home สามารถบรรลุเป้าหมายต่อไปนี้



Objective Description

Standardized Operations ทำให้ทุกสาขาทำงานด้วยมาตรฐานเดียวกัน

Better Care Documentation บันทึกข้อมูลการดูแลผู้พักอย่างเป็นระบบ

Real-time Management Visibility ผู้บริหารเห็นตัวเลขและสถานะสาขาแบบ Real-time

Revenue Control ควบคุมรายได้ บิล ลูกหนี้ และการชำระเงิน

Cost Control ติดตามต้นทุน อาหาร เวชภัณฑ์ บุคลากร และค่าใช้จ่าย

Quality Control ตรวจสอบคุณภาพบริการ เหตุการณ์ผิดปกติ และข้อร้องเรียน

Multi-Branch Scalability รองรับการขยายสาขาจำนวนมาก

Investor \& Bank Reporting สร้างรายงานสำหรับนักลงทุน ธนาคาร และผู้ถือหุ้น



⸻



3\. Target Users



User Role Main Responsibility

Super Admin ตั้งค่าระบบทั้งหมด ดูข้อมูลทุกสาขา

Head Office Management ดู Dashboard สาขา รายได้ EBITDA KPI และ Quality

Branch Manager บริหารงานประจำวันของสาขา

Nurse Manager ดูแลข้อมูลสุขภาพ แผนการดูแล ยา และ Incident

Caregiver / Nursing Assistant บันทึกกิจกรรมการดูแลรายวัน

Sales / Admission Officer จัดการ Lead, Site Visit, Quotation และ Admission

Finance / Accounting ออกบิล รับชำระเงิน ติดตามลูกหนี้ รายงานรายได้

HR / Admin จัดตารางเวร พนักงาน ขาด ลา มาสาย OT

Inventory / Procurement จัดการ Stock เวชภัณฑ์ อาหาร และของใช้

Family Member ดูข้อมูลบางส่วนของผู้พัก ใบแจ้งหนี้ และ Update จากศูนย์



⸻



4\. System Scope



4.1 MVP Scope — Phase 1



ระบบเวอร์ชันแรกควรประกอบด้วย Module หลักต่อไปนี้



Module Priority

User Login \& Permission Must Have

Branch / Room / Bed Setup Must Have

Patient Profile Must Have

Admission Management Must Have

Daily Care Record Must Have

Nursing Record Must Have

Medication Management Must Have

Incident Report Must Have

Staff Scheduling Must Have

Billing \& Payment Must Have

Branch Dashboard Must Have

Head Office Dashboard Must Have

Basic Reports Must Have



4.2 Phase 2 Scope



Module Priority

CRM / Lead Management Should Have

Inventory Management Should Have

Procurement Request Should Have

Family Portal Should Have

LINE / Email Notification Should Have

HR Records Should Have

Quality Audit Checklist Should Have

Expense Management Should Have



4.3 Phase 3 Scope



Module Priority

Accounting Integration Nice to Have

Payroll Integration Nice to Have

BI Dashboard Nice to Have

Investor Reporting Nice to Have

Bank Reporting Nice to Have

Franchise Dashboard Nice to Have

Mobile App Nice to Have

AI Care Risk Alert Future Enhancement



⸻



5\. Core ERP Modules



⸻



Module 1: User Login \& Permission Management



Purpose



ควบคุมการเข้าใช้งานระบบตามตำแหน่ง หน้าที่ และสาขา



Key Functions



\* Login / Logout

\* Password reset

\* Role-based access control

\* Branch-level access control

\* User creation

\* User suspension

\* Activity log

\* 2FA for management users



User Roles



Role Access Level

Super Admin All access

Head Office All branch dashboard and reports

Branch Manager Own branch only

Nurse Manager Own branch patient care data

Caregiver Daily care record only

Finance Billing and payment

HR Staff and schedule

Family Own family member only



Acceptance Criteria



\* Users can only access modules allowed by their role.

\* Branch users cannot see data from other branches.

\* Family users can only see their own patient’s selected information.

\* All login and data changes must be recorded in audit log.



⸻



Module 2: Branch / Room / Bed Management



Purpose



ใช้ตั้งค่าโครงสร้างสาขา ห้อง และเตียง เพื่อคำนวณ Occupancy และสถานะผู้พัก



Key Data Fields



Field Description

Branch ID รหัสสาขา

Branch Name ชื่อสาขา

Address ที่อยู่

Province จังหวัด

Branch Manager ผู้จัดการสาขา

Total Rooms จำนวนห้อง

Total Beds จำนวนเตียง

License Status สถานะใบอนุญาต

Opening Date วันที่เปิดสาขา

Branch Status Active / Inactive



Room Fields



Field Description

Room ID รหัสห้อง

Room Number เลขห้อง

Room Type Shared / Private / Premium

Floor ชั้น

Number of Beds จำนวนเตียง

Room Status Available / Occupied / Maintenance



Bed Fields



Field Description

Bed ID รหัสเตียง

Bed Number เลขเตียง

Room ID ห้องที่เตียงอยู่

Bed Status Available / Occupied / Reserved / Maintenance

Current Patient ผู้พักปัจจุบัน



Acceptance Criteria



\* System can calculate occupancy by branch.

\* Bed status updates automatically when patient is admitted or discharged.

\* Branch manager can see available beds in real time.



⸻



Module 3: Patient Profile



Purpose



เก็บข้อมูลพื้นฐาน ข้อมูลสุขภาพ ข้อมูลครอบครัว และข้อมูลการดูแลของผู้พัก



Key Data Fields



Category Fields

Personal Information ชื่อ, นามสกุล, ชื่อเล่น, เพศ, วันเกิด, อายุ, เลขบัตรประชาชน / Passport

Contact Information ที่อยู่เดิม, เบอร์โทร, Line ID

Family Contact ชื่อญาติ, ความสัมพันธ์, เบอร์โทร, Email, ผู้รับผิดชอบค่าใช้จ่าย

Medical Information โรคประจำตัว, ประวัติแพ้ยา, ประวัติแพ้อาหาร, โรงพยาบาลประจำ

Care Information Care Level, Mobility Status, Cognitive Status, Fall Risk, Feeding Support

Admission Information วันที่เข้าพัก, สาขา, ห้อง, เตียง, Package, ราคา

Document สำเนาบัตร, สัญญา, Consent Form, ใบรับรองแพทย์, Medication List



Care Level



Level Description

Low Care ช่วยเหลือตัวเองได้บางส่วน

Medium Care ต้องการผู้ช่วยในกิจวัตรประจำวัน

High Care ต้องดูแลใกล้ชิด เคลื่อนไหวลำบาก

Special Care มีเงื่อนไขพิเศษ เช่น Dementia, Stroke, Bedridden



Acceptance Criteria



\* Patient profile can be created and edited.

\* Patient can be assigned to branch, room, and bed.

\* Medical and family data are clearly separated.

\* Sensitive medical data must be permission-controlled.

\* Documents can be uploaded and viewed by authorized users.



⸻



Module 4: Admission Management



Purpose



บริหารกระบวนการรับผู้พักใหม่ ตั้งแต่ประเมินเบื้องต้นจนถึงเข้าพักจริง



Workflow



1\. Create Admission Case

2\. Enter family and patient details

3\. Record initial assessment

4\. Select branch / room / bed

5\. Select care package

6\. Generate quotation

7\. Upload documents

8\. Confirm deposit / payment

9\. Approve admission

10\. Convert to active patient



Admission Status



Status Meaning

Inquiry มีการสอบถาม

Assessment อยู่ระหว่างประเมิน

Quotation Sent ส่งราคาแล้ว

Pending Documents รอเอกสาร

Pending Payment รอชำระเงิน

Confirmed ยืนยันเข้าพัก

Active Patient เข้าพักแล้ว

Cancelled ยกเลิก



Required Documents



\* ID card / Passport

\* Family responsible person ID

\* Medical history

\* Medication list

\* Consent form

\* Service agreement

\* Payment evidence



Acceptance Criteria



\* Patient cannot become active unless required fields are completed.

\* Bed becomes reserved or occupied when admission is confirmed.

\* Quotation can be generated from selected care package.

\* Admission history is searchable.



⸻



Module 5: Daily Care Record



Purpose



ให้ Caregiver บันทึกกิจกรรมการดูแลประจำวันของผู้พักแต่ละราย



Daily Care Categories



Category Example Data

Wake Up / Sleep เวลาตื่น, เวลานอน, คุณภาพการนอน

Bathing อาบน้ำแล้ว / ยังไม่ได้ / ต้องช่วยเหลือ

Toileting ปกติ / ท้องผูก / ท้องเสีย / กลั้นไม่ได้

Meals กินได้หมด / กินได้น้อย / ไม่กิน / ต้องป้อน

Hydration ดื่มน้ำเพียงพอ / น้อย

Mobility เดินเอง / Walker / Wheelchair / Bedridden

Mood ปกติ / ซึม / หงุดหงิด / สับสน

Activity เข้าร่วมกิจกรรม / ไม่เข้าร่วม

Special Note หมายเหตุเพิ่มเติม



Shift Structure



Shift Time

Morning Shift 07:00–15:00

Afternoon Shift 15:00–23:00

Night Shift 23:00–07:00



System Features



\* Checklist format

\* Mobile / tablet friendly

\* Timestamp

\* Staff name auto-recorded

\* Supervisor review

\* Photo upload, if required

\* Flag abnormal observation



Acceptance Criteria



\* Caregiver can complete daily record within 1–2 minutes per patient.

\* Nurse Manager can review incomplete records.

\* Branch Manager can see daily completion rate.

\* System alerts if care record is missing by end of shift.



⸻



Module 6: Nursing Record



Purpose



ให้ Nurse Manager บันทึกข้อมูลด้านสุขภาพ อาการสำคัญ Vital Signs และการติดตามอาการ



Data Fields



Field Description

Date / Time วันเวลา

Patient Name ชื่อผู้พัก

Blood Pressure ความดัน

Pulse ชีพจร

Temperature อุณหภูมิ

Oxygen Saturation ค่าออกซิเจน

Blood Sugar น้ำตาล หากเกี่ยวข้อง

Pain Score ระดับความเจ็บปวด

Wound Care การดูแลแผล

Symptoms อาการผิดปกติ

Nursing Note บันทึกพยาบาล

Follow-up Action การติดตาม

Nurse Name ผู้บันทึก



Acceptance Criteria



\* Nurse can record vital signs and nursing notes.

\* System can show trend by patient.

\* Abnormal values can be flagged.

\* Records can be exported for hospital referral.



⸻



Module 7: Medication Management



Purpose



ควบคุมรายการยา ตารางยา การให้ยา และลดความผิดพลาดจากการให้ยา



Medication Master Fields



Field Description

Medication Name ชื่อยา

Dosage ขนาดยา

Frequency ความถี่

Time เวลาให้ยา

Route Oral / Injection / External

Start Date วันที่เริ่ม

End Date วันที่สิ้นสุด

Prescribing Doctor แพทย์ผู้สั่ง

Special Instruction คำแนะนำ

High Risk Flag ยาความเสี่ยงสูง



Medication Administration Status



Status Meaning

Given ให้ยาแล้ว

Not Given ไม่ได้ให้

Refused ผู้พักปฏิเสธ

Delayed ให้ล่าช้า

Held by Doctor แพทย์สั่งหยุด



Alerts



\* Upcoming medication

\* Missed medication

\* Medication expiry

\* High-risk medication

\* Medication changed



Acceptance Criteria



\* System shows medication schedule by patient and shift.

\* Staff must record status for each medication time.

\* Missed dose must require reason.

\* Nurse Manager can review medication compliance.



⸻



Module 8: Incident Management



Purpose



บันทึกเหตุการณ์ผิดปกติ ลดความเสี่ยง และสร้างหลักฐานการจัดการปัญหา



Incident Types



Type Example

Fall ผู้พักหกล้ม

Injury บาดเจ็บ

Medication Error ให้ยาผิด / ลืมให้ยา

Choking สำลัก

Abnormal Symptom อาการผิดปกติ

Complaint ข้อร้องเรียน

Emergency Transfer ส่งโรงพยาบาล

Missing Item ทรัพย์สินสูญหาย

Aggressive Behavior พฤติกรรมก้าวร้าว

Death เสียชีวิต



Workflow



1\. Staff reports incident.

2\. Nurse Manager reviews.

3\. Branch Manager confirms.

4\. Family notification is recorded.

5\. Corrective action is assigned.

6\. Head Office reviews serious inciden


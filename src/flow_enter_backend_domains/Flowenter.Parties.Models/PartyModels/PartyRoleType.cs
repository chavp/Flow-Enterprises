using Flowenter.Domain.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Flowenter.Parties.Models.PartyModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class PartyRoleType: BaseEntity   
{
    public const string Enterprise = "ENTERPRISE";
    public const string Branch = "BRANCH";

    public const string Customer = "CUSTOMER";
    public const string Employee = "EMPLOYEE";

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }

    //Nursing Home Roles: บทบาทหน้าที่ในสถานดูแลผู้สูงอายุ

    public const string Administrator = "ADMINISTRATOR"; // ผู้บริหาร / ผู้จัดการสถานดูแล
    public const string CareManager = "CARE_MANAGER"; //ผู้จัดการดูแลผู้ป่วย / ผู้จัดการเคส
    public const string Nurse = "NURSE";//พยาบาล
    public const string Caregiver = "CAREGIVER";//ผู้ดูแล
    public const string Physician = "PHYSICIAN";//แพทย์
    public const string Pharmacist = "PHARMACIST";//เภสัชกร
    public const string Dietitian = "DIETITIAN"; //นักกำหนดอาหาร / นักโภชนาการ
    public const string KitchenStaff = "KITCHEN_STAFF"; //พนักงานครัว
    public const string HousekeepingStaff = "HOUSEKEEPING_STAFF"; //พนักงานทำความสะอาด
    public const string MaintenanceStaff = "MAINTENANCE_STAFF"; //ช่างซ่อมบำรุง
    public const string LaundryStaff = "LAUNDRY_STAFF"; //พนักงานซักรีด
    public const string Receptionist = "RECEPTIONIST"; //พนักงานต้อนรับ
    public const string Patient = "PATIENT"; //ผู้ป่วย
    public const string SecurityGuard = "SECURITY_GUARD"; //เจ้าหน้าที่รักษาความปลอดภัย
}

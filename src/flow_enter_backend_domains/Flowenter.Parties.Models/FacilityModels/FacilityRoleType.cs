using Flowenter.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Flowenter.Parties.Models.FacilityModels;

[Index(nameof(Code), IsUnique = true)]
public sealed class FacilityRoleType : BaseEntity
{
    public const string Own = "OWN"; //ความเป็นเจ้าของ ,  ใช้เพื่อระบุว่าบุคคลหรือองค์กรใดเป็นเจ้าของสถานที่หรือสิ่งปลูกสร้างนั้นๆ
    public const string Rent = "Rent"; //การเช่า, สำหรับระบุผู้ที่เข้ามาเช่าสถานที่เพื่อใช้งานในลักษณะต่างๆ
    public const string Lease = "Lease"; //การเช่าระยะยาว, สำหรับบันทึกคู่สัญญาที่ทำสัญญาเช่าสถานที่ในระยะเวลาที่กำหนด
    public const string Use = "Use"; //การใช้งาน, ใช้ระบุฝ่ายที่เข้ามาใช้พื้นที่ปฏิบัติงานจริง ซึ่งอาจเป็นหน่วยงานภายในที่ได้รับมอบหมายให้ใช้พื้นที่นั้นๆ

    [Required, StringLength(100)]
    public string? Code { get; set; }

    [Required, StringLength(200)]
    public string? Name { get; set; }
}

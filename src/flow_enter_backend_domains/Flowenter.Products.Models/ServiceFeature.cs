using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Flowenter.Products.Models;

//[Index(nameof(ServiceProviderPartyId), nameof(Code), IsUnique = true)]
public class ServiceFeature : ProductFeature
{

    //// Daily Living & Wellness, เน้นการใช้ชีวิตประจำวันและคุณภาพชีวิต
    //public const string RecreationalActivitiesProgram = "RECRE_ACT_PROG"; // โปรแกรมสันทนาการ
    //public const string TreeNutritionallyBalancedMealsPerDay = "TREE_NUT_BALAN_PD"; // อาหาร 3 มื้อตามหลักโภชนาการ
    //public const string HousekeepingAndLaundryServices = "HKEEP_N_LANDRY_SERV"; // มีพนักงานดูแลความสะอาดและซักรีด

    //// Nursing & Health Monitoring, เน้นความปลอดภัยและการติดตามอาการทางการแพทย์อย่างต่อเนื่อง
    //public const string DailyVitalSignsMonitoring = "RECRE_ACT_PROG"; // ตรวจเช็กสัญญาณชีพรายวัน
    //public const string MedicationManagementByProfessionalNurses = "RECRE_ACT_PROG"; // บริหารจัดการยาโดยพยาบาลวิชาชีพ
    //public const string TwentyFourhourProfessionalNursingCare = "RECRE_ACT_PROG"; // การดูแล 24 ชม. โดยทีมพยาบาล
    //public const string ComprehensiveHealthStatusReportingViaMobileApplication = "RECRE_ACT_PROG"; // รายงานอาการละเอียดผ่านแอปฯ

    //// Rehabilitation & Specialized Care, เน้นการฟื้นฟูสมรรถภาพและการป้องกันภาวะแทรกซ้อนสำหรับผู้ป่วย
    //public const string BasicPhysicalTherapy = "RECRE_ACT_PROG"; // กายภาพบำบัดพื้นฐานสัปดาห์ละ 2 ครั้ง
    //public const string IntensivePhysicalTherapy = "RECRE_ACT_PROG"; // การทำกายภาพบำบัดเข้มข้น (Intensive PT)
    //public const string PatientRepositioningToPreventPressureUlcers = "RECRE_ACT_PROG"; // การพลิกตัวป้องกันแผลกดทับ

}

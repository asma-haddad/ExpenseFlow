using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Infrastructure.Seeder
{
    public static class PermissionGroups
    {
        public static readonly PermissionType[] Employee =
  {
        PermissionType.ExpenseViewOwn,
        PermissionType.ExpenseCreate,
        PermissionType.ExpenseEditOwnDraft,
        PermissionType.ExpenseDeleteOwnDraft,
        PermissionType.ExpenseSubmit
    };


        public static readonly PermissionType[] Manager =
        {
        PermissionType.ExpenseViewDepartment,
        PermissionType.ExpenseApprove,
        PermissionType.ExpenseReject
    };


        public static readonly PermissionType[] Finance =
        {
        PermissionType.ExpenseViewApproved,
        PermissionType.ExpenseMarkAsPaid,
        PermissionType.ExpenseViewReports
    };


        public static readonly PermissionType[] Admin =
        {
        PermissionType.UserManage,
        PermissionType.RoleManage,
        PermissionType.PermissionManage,
        PermissionType.DepartmentManage,
        PermissionType.ExpenseCategoryManage,
        PermissionType.SystemSettingManage
    };


        public static PermissionType[] GetManagerPermissions()
        {
            return Employee
                .Concat(Manager)
                .Distinct()
                .ToArray();
        }


        public static PermissionType[] GetFinancePermissions()
        {
            return Employee
                .Concat(Finance)
                .Distinct()
                .ToArray();
        }


        public static PermissionType[] GetAdminPermissions()
        {
            return Enum.GetValues<PermissionType>();
        }

    }
}

using ExpenseFlow.Domain.Shared.Enum;

namespace ExpenseFlow.Infrastructure.Seeder
{
    public static class PermissionGroups
    {
        // Employee Permissions
        public static readonly PermissionType[] Employee =
        {
        PermissionType.ExpenseViewOwn,
        PermissionType.ExpenseCreate,
        PermissionType.ExpenseEditOwnDraft,
        PermissionType.ExpenseDeleteOwnDraft,
        PermissionType.ExpenseSubmit
    };


        // Manager-only Permissions
        public static readonly PermissionType[] Manager =
        {
        PermissionType.ExpenseViewDepartment,
        PermissionType.ExpenseApprove,
        PermissionType.ExpenseReject
    };


        // Finance-only Permissions
        public static readonly PermissionType[] Finance =
        {
        PermissionType.ExpenseViewApproved,
        PermissionType.ExpenseMarkAsPaid,
        PermissionType.ExpenseViewReports
    };


        public static PermissionType[] GetEmployeePermissions()
        {
            return Employee;
        }


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

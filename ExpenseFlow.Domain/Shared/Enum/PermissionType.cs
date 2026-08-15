namespace ExpenseFlow.Domain.Shared.Enum
{
    public enum PermissionType
    {
        // Employee
        ExpenseViewOwn = 1001,
        ExpenseCreate = 1002,
        ExpenseEditOwnDraft = 1003,
        ExpenseDeleteOwnDraft = 1004,
        ExpenseSubmit = 1005,

        // Manager
        ExpenseViewDepartment = 2001,
        ExpenseApprove = 2002,
        ExpenseReject = 2003,

        // Finance
        ExpenseViewApproved = 3001,
        ExpenseMarkAsPaid = 3002,
        ExpenseViewReports = 3003,

        // Admin
        UserManage = 4001,
        RoleManage = 4002,
        PermissionManage = 4003,
        DepartmentManage = 4004,
        ExpenseCategoryManage = 4005,
        SystemSettingManage = 4006
    }
}

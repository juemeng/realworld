namespace Moniter.Infrastructure.Errors
{
    public static class ErrorMessages
    {
        public const string UserNameInUse = @"用户名已存在";
        public const string EmailInUse = @"邮箱地址已存在";
        public const string UserCastFailure = @"用户模型转换失败，请检查提交的JSON数据";
        public const string PasswordCannotBeNull = @"密码不能为空";
        public const string PasswordLengthGreaterThan = @"密码长度必须大于6位";
        public const string PasswordLengthLessThan = @"密码长度必须小于16位";
        public const string UserNameCannotBeNull = @"用户名不能为空";
        public const string UserNotFound = @"用户未找到";
        public const string EmailCannotBeNull = @"邮箱地址不能为空";
        public const string EmailFormatError = @"邮箱格式不正确";
    }
}

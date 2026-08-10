using ExpenseFlow.Domain.Shared.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace ExpenseFlow.Domain.Base
{
    public class Result
    {
        public string ErrorMessage { get; set; }
        public ResultStatus Status { get; set; }

        public Result(ResultStatus operationStatus = ResultStatus.Success)
        {
            Status = operationStatus;
            ErrorMessage = string.Empty;
        }
        public void AddError(string errorMessage, ResultStatus status)
        {
            ErrorMessage = errorMessage;
            Status = status;
        }
        public void ThrowException(string errorMessage, ResultStatus status)
        {
            switch (status)
            {
                case ResultStatus.Failed:
                    throw new InternalServerException(errorMessage);
                case ResultStatus.NotFound:
                    throw new NotFoundException(errorMessage);
                case ResultStatus.ValidationError:
                    throw new BadRequestException(errorMessage);
                case ResultStatus.InternalServerError:
                    throw new InternalServerException(errorMessage);
                case ResultStatus.UnAuthenticated:
                    throw new UnAuthorizedException(errorMessage.ToString());
                default:
                    throw new InternalServerException(errorMessage);
            }

        }


    }

    public class Result<TEntity> : Result
    {
        public TEntity Data { get; set; }

        public Result(ResultStatus operationStatus = ResultStatus.Success)
        {
            Status = operationStatus;
            ErrorMessage = string.Empty;
        }

        public static Result<TEntity> Success(TEntity data) =>
            new(ResultStatus.Success) { Data = data };
    }

    public static class ResultExtentions
    {
        public static JsonResult GetResult<TEntity>(this Result<TEntity> result)
        {

            switch (result.Status)
            {
                case ResultStatus.Success:
                    return new JsonResult(result.Data);
                case ResultStatus.Failed:
                    throw new InternalServerException(result.ErrorMessage.ToString());
                case ResultStatus.NotFound:
                    throw new NotFoundException(result.ErrorMessage.ToString());
                case ResultStatus.ValidationError:
                    throw new BadRequestException(result.ErrorMessage.ToString());
                case ResultStatus.InternalServerError:
                    throw new InternalServerException(result.ErrorMessage.ToString());
                default:
                    throw new InternalServerException(result.ErrorMessage.ToString());
            }
        }

        public static StatusCodeResult GetResult(this Result result)
        {

            switch (result.Status)
            {
                case ResultStatus.Success:
                    return new NoContentResult();
                case ResultStatus.Failed:
                    throw new InternalServerException(result.ErrorMessage.ToString());
                case ResultStatus.NotFound:
                    throw new NotFoundException(result.ErrorMessage.ToString());
                case ResultStatus.ValidationError:
                    throw new BadRequestException(result.ErrorMessage.ToString());
                case ResultStatus.InternalServerError:
                    throw new InternalServerException(result.ErrorMessage.ToString());
                case ResultStatus.UnAuthenticated:
                    throw new AuthenticationException(result.ErrorMessage.ToString());
                default:
                    throw new InternalServerException(result.ErrorMessage.ToString());
            }
        }
        public static Result<TResponse> FailIf<TResponse>(this Result<TResponse> result, bool predicate, string errorMessage = null, ResultStatus status = ResultStatus.ValidationError)
        {
            if (predicate)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Validation error" : errorMessage, status);
                return result;
            }
            return result;
        }

        public static Result FailIf(this Result result, bool predicate, string errorMessage = null, ResultStatus status = ResultStatus.ValidationError)
        {
            if (predicate)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Validation error" : errorMessage, status);
                return result;
            }
            return result;
        }

        public static Result<TResponse> FailIfNullOrEmpty<TResponse>(this Result<TResponse> result, object obj, string errorMessage = null, ResultStatus status = ResultStatus.ValidationError)
        {
            if (obj == null)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Some fields is required" : errorMessage, status);
                return result;
            }
            else if (obj.GetType() == typeof(Guid) && (Guid)obj == Guid.Empty)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Some fields is required" : errorMessage, status);
                return result;
            }
            else if (obj.GetType() == typeof(string) && string.IsNullOrEmpty(obj.ToString()))
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Some fields is required" : errorMessage, status);
                return result;
            }
            else if (obj.GetType().GetInterfaces().Any(s => s == typeof(ICollection)) && ((ICollection<object>)obj).Count == 0)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "List can't be empty" : errorMessage, status);
                return result;
            }
            return result;
        }

        public static Result FailIfNullOrEmpty(this Result result, object obj, string errorMessage = null, ResultStatus status = ResultStatus.ValidationError)
        {
            if (obj == null)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Some fields is required" : errorMessage, status);
                return result;
            }
            else if (obj.GetType() == typeof(Guid) && (Guid)obj == Guid.Empty)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Some fields is required" : errorMessage, status);
                return result;
            }
            else if (obj.GetType() == typeof(string) && string.IsNullOrEmpty(obj.ToString()))
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "Some fields is required" : errorMessage, status);
                return result;


            }
            else if (obj.GetType().GetInterfaces().Any(s => s == typeof(ICollection)) && ((ICollection)obj).Count == 0)
            {
                result.ThrowException(string.IsNullOrEmpty(errorMessage) ? "List can't be empty" : errorMessage, status);
                return result;
            }
            return result;
        }


    }


}

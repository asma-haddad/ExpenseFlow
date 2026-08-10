using ExpenseFlow.Domain.Base.Dto;
using ExpenseFlow.Domain.Base.Language;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpenseFlow.Application.Extensions
{
    public static class DataExtensions
    {

        public static async Task<GetAllDataResponse<TEntity>> PaginateAsync<TEntity>(this IQueryable<TEntity> data,
            PaginationRequest request, CancellationToken cancellationToken = default)
        {
            // Single async count instead of a blocking Any() + Count() (3 round-trips -> 2).
            var count = await data.CountAsync(cancellationToken);

            if (count > 0 && request.PageSize != 0)
            {
                data = data.Skip(request.PageSize * request.PageNumber).Take(request.PageSize);
            }

            return new GetAllDataResponse<TEntity>
            {
                Data = count == 0 ? new List<TEntity>() : await data.ToListAsync(cancellationToken),
                PageNumber = request.PageNumber,
                TotalPages = request.PageSize == 0 ? 1 : (count + request.PageSize - 1) / request.PageSize,
                TotalDataCount = count
            };
        }

        /// <summary>
        /// Paginates by counting the source (entity) query <b>before</b> the projection,
        /// then applies Skip/Take and projects only the current page. Cheaper than
        /// counting an already-projected query that carries subqueries / ToDto calls.
        /// </summary>
        public static async Task<GetAllDataResponse<TResult>> PaginateAsync<TSource, TResult>(
            this IQueryable<TSource> source,
            Expression<Func<TSource, TResult>> selector,
            PaginationRequest request,
            CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);

            List<TResult> items;
            if (count == 0)
            {
                items = new List<TResult>();
            }
            else
            {
                var paged = request.PageSize != 0
                    ? source.Skip(request.PageSize * request.PageNumber).Take(request.PageSize)
                    : source;
                items = await paged.Select(selector).ToListAsync(cancellationToken);
            }

            return new GetAllDataResponse<TResult>
            {
                Data = items,
                PageNumber = request.PageNumber,
                TotalPages = request.PageSize == 0 ? 1 : (count + request.PageSize - 1) / request.PageSize,
                TotalDataCount = count
            };
        }

        public static GetAllDataResponse<TEntity> Paginate<TEntity>(this IEnumerable<TEntity> data,
        PaginationRequest request)
        {
            var count = 0;
            if (data.Any())
            {
                count = data.Count();
                if (request.PageSize != 0)
                {
                    data = data.Skip(request.PageSize * request.PageNumber).Take(request.PageSize);
                }
            }

            return new GetAllDataResponse<TEntity>
            {
                Data = data.ToList(),
                PageNumber = request.PageNumber,
                TotalPages = request.PageSize == 0 ? 1 : (count + request.PageSize - 1) / request.PageSize,
                TotalDataCount = count
            };
        }
        public static IOrderedQueryable<T> SortBy<T>(
            this IQueryable<T> source, string? propertyName, bool isAscending, string? sortLanguage)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return (IOrderedQueryable<T>)source;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression propertyAccess = parameter;

            foreach (var property in propertyName.Split('.'))
            {
                propertyAccess = Expression.Property(propertyAccess, property);
            }

            Expression lambdaBody;

            if (propertyAccess.Type == typeof(LanguagePropertyModel) && !string.IsNullOrWhiteSpace(sortLanguage))
            {
                var getValueByLangMethod = typeof(LanguagePropertyModelExtension)
                    .GetMethod(nameof(LanguagePropertyModelExtension.ToDto), new[] { typeof(LanguagePropertyModel), typeof(string) });

                if (getValueByLangMethod == null)
                {
                    throw new InvalidOperationException("DbFunction 'GetValueByLang' not found.");
                }

                var langCodeExpression = Expression.Constant(sortLanguage);

                lambdaBody = Expression.Call(
                    null,
                    getValueByLangMethod,
                    propertyAccess,
                    langCodeExpression
                );
            }
            else
            {
                lambdaBody = propertyAccess;
            }

            var lambda = Expression.Lambda(lambdaBody, parameter);

            string methodName = isAscending ? "OrderBy" : "OrderByDescending";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), lambdaBody.Type },
                source.Expression,
                Expression.Quote(lambda)
            );

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(resultExpression);
        }

        public static IOrderedQueryable<TSource> Ordering<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool isDescending)
        {
            return isDescending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
        }

        public static IOrderedQueryable<TSource> ThenOrderingBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool isDescending)
        {
            return isDescending ? source.ThenByDescending(keySelector) : source.ThenBy(keySelector);
        }

        private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return null;
            }

            var parts = propertyName.Split('.');
            var currentType = type;
            PropertyInfo propertyInfo = null;

            foreach (var part in parts)
            {
                propertyInfo = currentType.GetProperty(part, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return null;
                }
                currentType = propertyInfo.PropertyType;
            }

            return propertyInfo;
        }
    }

}


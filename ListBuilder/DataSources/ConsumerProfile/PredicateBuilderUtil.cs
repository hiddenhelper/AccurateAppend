using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.ListBuilder.Models;
using DAL.Databases;

namespace AccurateAppend.ListBuilder.DataSources.ConsumerProfile
{
    public static class PredicateBuilderUtil
    {
        /// <summary>
        /// Returns a queryable containing all predicated for the input <paramref name="criteria"/>
        /// </summary>
        public static IQueryable<Profile> GetPredicate(IQueryable<Profile> source, ListCriteria criteria)
        {
            // profile =>
            var pe = Expression.Parameter(typeof(Profile), "profile");
            IList<BinaryExpression> orExpressions = new List<BinaryExpression>();
            IList<BinaryExpression> andExpressions = new List<BinaryExpression>();

            if (criteria.States.Any())
            {
                orExpressions.AddRange(States(pe, criteria.States.ToArray()));
            }

            if (criteria.Counties.Any())
            {
                orExpressions.AddRange(County(pe, criteria.Counties.ToArray()));
            }

            if (criteria.Cities.Any())
            {
                orExpressions.AddRange(City(pe, criteria.Cities.ToArray()));
            }

            if (criteria.Zips.Any())
            {
                orExpressions.AddRange(Zip(pe, criteria.Zips.ToArray()));
            }

            if (criteria.DobRanges.Any())
            {
                andExpressions.AddRange(DobRange(pe, criteria.DobRanges.ToArray()));
            }

            //if (criteria.Hoh.Any())
            //{
            //    andExpressions.AddRange(Hoh(pe, criteria.Hoh.ToArray()));
            //}

            //if (criteria.Homeowners.Any())
            //{
            //    andExpressions.AddRange(Homeowner(pe, criteria.Homeowners.ToArray()));
            //}

            Expression predicateGroup1 = null;

            if (orExpressions.Any())
            {
                foreach (var expression in orExpressions)
                {
                    if (predicateGroup1 == null)
                    {
                        predicateGroup1 = expression;
                    }
                    else
                    {
                        predicateGroup1 = Expression.OrElse(predicateGroup1, expression);
                    }
                }
            }

            Expression predicateGroup2 = null;

            if (andExpressions.Any())
            {
                foreach (var expression in andExpressions)
                {
                    if (predicateGroup2 == null)
                    {
                        predicateGroup2 = expression;
                    }
                    else
                    {
                        predicateGroup2 = Expression.OrElse(predicateGroup2, expression);
                    }
                }
            }

            Expression predicateBody = null;
            if (predicateGroup1 != null && predicateGroup2 != null) predicateBody = Expression.AndAlso(predicateGroup1, predicateGroup2);
            if (predicateGroup1 == null || predicateGroup2 == null) predicateBody = predicateGroup1 ?? predicateGroup2;
            if (predicateBody == null) predicateBody = Expression.Equal(Expression.Constant(true), Expression.Constant(false));

            var whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new[] { source.ElementType },
                    source.Expression,
                    Expression.Lambda<Func<Profile, Boolean>>(predicateBody, new[] { pe }));

            var query = source.Provider.CreateQuery<Profile>(whereCallExpression);

            return query;
        }

        /// <summary>
        /// Creates individual expressions for a collection of <see cref="State"/>.
        /// </summary>
        private static IList<BinaryExpression> States(ParameterExpression pe, params global::AccurateAppend.ListBuilder.Models.State[] states)
        {
            var left = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.FIPS_ST)));

            var expressions = new List<BinaryExpression>();

            foreach (var state in states)
            {
                var tempState = state.Fips;
                var right = Expression.Constant(tempState, typeof(Int32?));
                var exp = Expression.Equal(left, right);

                expressions.Add(exp);
            }

            return expressions;
        }
        
        /// <summary>
        /// Creates individual expressions for a collection of <see cref="ListBuilder.Models.Zip"/>.
        /// </summary>
        private static IList<BinaryExpression> Zip(ParameterExpression pe, params Zip[] zips)
        {
            var left = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.ZIP)));

            var expressions = new List<BinaryExpression>();

            foreach (var zip in zips)
            {
                var tempZip = zip.Name;
                var right = Expression.Constant(tempZip);
                var exp = Expression.Equal(left, right);

                expressions.Add(exp);
            }

            return expressions;
        }

        /// <summary>
        /// Creates individual expressions for a collection of <see cref="County"/>.
        /// </summary>
        /// <param name="counties"></param>
        /// <returns></returns>
        private static IList<BinaryExpression> County(ParameterExpression pe, params County[] counties)
        {
            var left = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.FIPS_CTY)));

            var expressions = new List<BinaryExpression>();

            foreach (var county in counties)
            {
                var tempFipsCounty = county.Fips;
                var tempFipsState = county.State;

                var right = Expression.Constant(tempFipsCounty, typeof(Int32?));
                var exp = Expression.Equal(left, right);
                exp = Expression.AndAlso(exp, States(pe, tempFipsState).First());

                expressions.Add(exp);
            }

            return expressions;
        }
        
        /// <summary>
        /// Creates individual expressions for a collection of <see cref="City"/>.
        /// </summary>
        private static IList<BinaryExpression> City(ParameterExpression pe, params City[] cities)
        {
            var left = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.CITY)));

            var expressions = new List<BinaryExpression>();

            foreach (var city in cities)
            {
                var tempFipsCityName = city.Name;
                var tempFipsState = city.State;

                var right = Expression.Constant(tempFipsCityName);
                var exp = Expression.Equal(left, right);
                exp = Expression.AndAlso(exp, States(pe, tempFipsState).First());

                expressions.Add(exp);
            }

            return expressions;
        }

        /// <summary>
        /// Creates indivdual expressions containing all dates for a collection of <see cref="ListBuilder.Models.DobRange"/>
        /// </summary>
        /// <param name="dobRanges"></param>
        /// <returns></returns>
        private static IList<BinaryExpression> DobRange(ParameterExpression pe, params DobRange[] dobRanges)
        {
            var yearLeft = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.DOB_YR)));
            var monthLeft = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.DOB_MON)));

            var expressions = new List<BinaryExpression>();

            foreach (var dobRange in dobRanges)
            {
                var start = new DateTime(Int32.Parse(dobRange.Start.Year), Int32.Parse(dobRange.Start.Month), 1);
                var end = new DateTime(Int32.Parse(dobRange.End.Year), Int32.Parse(dobRange.End.Month), 1);
                var dates = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                    .Select(offset => start.AddDays(offset))
                    .Select(a => new { a.Month, a.Year })
                    .Distinct()
                    .ToArray();

                foreach (var date in dates)
                {
                    var tempYear = date.Year.ToString();
                    var yearRight = Expression.Constant(tempYear);
                    var exp1 = Expression.Equal(yearLeft, yearRight);

                    var tempMonth = date.Month.ToString("D2");
                    var monthRight = Expression.Constant(tempMonth);
                    var exp2 = Expression.Equal(monthLeft, monthRight);

                    var exp = Expression.AndAlso(exp1, exp2);

                    expressions.Add(exp);
                }
            }

            return expressions;
        }

        /// <summary>
        /// Creates individual expressions for a collection of <see cref="Hoh"/>.
        /// </summary>
        private static IList<BinaryExpression> Hoh(ParameterExpression pe, params Hoh[] hohs)
        {
            var left = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.INF_HH_RANK)));

            var expressions = new List<BinaryExpression>();

            foreach (var hoh in hohs)
            {
                var tempRank = hoh.Rank;

                var right = Expression.Constant(tempRank, typeof(int?));
                var exp = Expression.Equal(left, right);
                expressions.Add(exp);
            }

            return expressions;
        }

        /// <summary>
        /// Creates individual expressions for a collection of <see cref="Homeowner"/>.
        /// </summary>
        private static IList<BinaryExpression> Homeowner(ParameterExpression pe, params Homeowner[] homeowners)
        {
            var left = Expression.Property(pe, typeof(Profile).GetProperty(nameof(Profile.HOME_OWNR)));

            var expressions = new List<BinaryExpression>();

            // select "All"
            if (homeowners.Any(a => a.IndicatorValue == ""))
            {
                var unknown = Expression.Equal(left, Expression.Constant(""));
                expressions.Add(unknown);
                var owner = Expression.Equal(left, Expression.Constant("O"));
                expressions.Add(owner);
                var renter = Expression.Equal(left, Expression.Constant("R"));
                expressions.Add(renter);
            }
            
            // select specific homeowners
            foreach (var homeowner in homeowners)
            {
                var tempIndicator = homeowner.IndicatorValue;

                var right = Expression.Constant(tempIndicator);
                var exp = Expression.Equal(left, right);
                expressions.Add(exp);
            }

            return expressions;
        }
    }
}

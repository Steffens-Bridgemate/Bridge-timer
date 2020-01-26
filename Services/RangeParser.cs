using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BridgeTimer.Services.RangeParsing
{
    public static class TextExtensions
    {

        public static IEnumerable<string> ExtractInvalidRanges(this string range, int minValue, int MaxValue)
        {
            //const string pattern = @"(^\s*(o|e)?\d+\s*$)|^(\s*(o|e)?\d+\s*-\s*\d*\s*$)|(^\s*(o|e)?-\s*\d+$)";
            const string pattern = @"(^\s*((o|e)?|\d+d\d+r)\d+\s*$)|^(\s*((o|e)?|\d+d\d+r)\d+\s*-\s*\d*\s*$)|(^\s*((o|e)?|\d+d\d+r)-\s*\d+$)";
            if (string.IsNullOrWhiteSpace(range))
                range = "";

            Regex regx = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var rangeParticles = range.Split(',');
            foreach (var particle in rangeParticles)
            {
                if (!regx.IsMatch(particle))
                    yield return particle;
                var atoms = particle.Split('-');
                foreach (var atom in atoms)
                {
                    var tempAtom = atom.Replace("o", "");
                    tempAtom = tempAtom.Replace("e", "");

                    int atomValue;
                    if (string.IsNullOrWhiteSpace(tempAtom))
                        continue;
                    var success = int.TryParse(tempAtom, out atomValue);
                    if (!success) continue;

                    if (atomValue < minValue || atomValue > MaxValue)
                        yield return particle;
                }
            }
        }

        public static IEnumerable<int> ParseToSortedCollection(this string range, int minValue, int maxValue)
        {
            var rangeValues = range.ParseToDistinctValues(minValue, maxValue).OrderBy(value => value);
            return rangeValues;
        }

        public static IEnumerable<int> ParseToDistinctValues(this string range, int minValue, int maxValue)
        {
            var rangeValues = range.ParseRange(minValue, maxValue).Distinct();
            return rangeValues;
        }

        /// <summary>
        /// Parses a string representing a range of values into a sequence of integers.
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="minValue">Minimum value for open range specifier</param>
        /// <param name="maxValue">Maximum value for open range specifier</param>
        /// <returns>An enumerable sequence of integers</returns>
        /// <remarks>
        /// The range is specified as a string in the following forms or combination thereof:
        /// 5           single value
        /// 1,2,3,4,5   sequence of values
        /// 1-5         closed range
        /// -5          open range (converted to a sequence from minValue to 5)
        /// 1-          open range (converted to a sequence from 1 to maxValue)
        /// 
        /// The value delimiter can be either ',' or ';' and the range separator can be
        /// either '-' or ':'. Whitespace is permitted at any point in the input.
        /// 
        /// Any elements of the sequence that contain non-digit, non-whitespace, or non-separator
        /// characters or that are empty are ignored and not returned in the output sequence.
        /// </remarks>
        private static IEnumerable<int> ParseRange(this string s, int minValue, int maxValue)
        {
            const string pattern = @"(?:^|(?<=[,;])) 	                  # match must begin with start of string or delim, where delim is , or ;
                                     \s*                                  # leading whitespace
                                     ((?<odd>(o|e)?)                      # capture a possibe 'filter on odd (o)' or 'filter on even (e)'element
                                      |                                   # or
                                      ((?<divide>\d+)d                    # capture a possible 'filter on remainder (#d#r)' element.
                                      (?<remainder>\d+)r)?)               #
                                     (            	                      # encloses the two possible valid range string.
                                     (?<from>\d*)\s*(?:-|:)\s*(?<to>\d+)  # capture 'from <sep> to' or '<sep> to', where <sep> is - or :
                                     |			                          # or
                                     (?<from>\d+)\s*(?:-|:)\s*(?<to>\d*)  # capture 'from <sep> to' or 'from <sep>', where <sep> is - or :
                                     |                                    # or
                                     (?<num>\d+)	                      # capture lone number
                                     )\s*              	                  # trailing whitespace
                                     (?:(?=[,;\b])|$)                     # match must end with end of string or delim, where delim is , or ;";

            if (s.ExtractInvalidRanges(minValue, maxValue).Any())
            {
                var empytList = new List<int>();
                foreach (var nonItem in empytList)
                    yield return nonItem;
            }
            else
            {

                Regex regx = new Regex(pattern, RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.IgnoreCase);

                foreach (Match m in regx.Matches(s))
                {
                    var oddOrEven = m.Groups["odd"];
                    bool? isOdd = null;

                    if (oddOrEven.Success)
                    {
                        if (oddOrEven.Value == "o") isOdd = true;
                        else if (oddOrEven.Value == "e") isOdd = false;
                    }

                    var divideGroup = m.Groups["divide"];
                    var remainderGroup = m.Groups["remainder"];
                    int? divide = null;
                    int remainder = 0;

                    if (divideGroup.Success)
                    {
                        divide = int.Parse(divideGroup.Value);
                        remainder = int.Parse(remainderGroup.Value);
                    }

                    Group gpNum = m.Groups["num"];
                    if (gpNum.Success)
                    {
                        var intValue = int.Parse(gpNum.Value);
                        if (isOdd.HasValue)
                        {
                            if (isOdd == true && intValue % 2 == 1)
                            {
                                yield return intValue;
                            }
                            else if (isOdd == false && intValue % 2 == 0)
                            {
                                yield return intValue;
                            }
                        }
                        else if (divide.HasValue && divide > 0)
                        {
                            if (intValue % divide == remainder)
                            {
                                yield return intValue;
                            }
                        }
                        else yield return intValue;

                    }
                    else
                    {
                        Group gpFrom = m.Groups["from"];
                        Group gpTo = m.Groups["to"];
                        if (gpFrom.Success || gpTo.Success)
                        {
                            int from = (gpFrom.Success && gpFrom.Value.Length > 0 ? int.Parse(gpFrom.Value) : minValue);
                            int to = (gpTo.Success && gpTo.Value.Length > 0 ? int.Parse(gpTo.Value) : maxValue);

                            for (int intValue = from; to >= from ? intValue <= to : intValue >= to; intValue = to >= from ? intValue + 1 : intValue - 1)
                            {
                                if (isOdd.HasValue)
                                {
                                    if (isOdd == true && intValue % 2 == 1)
                                    {
                                        yield return intValue;
                                    }
                                    else if (isOdd == false && intValue % 2 == 0)
                                    {
                                        yield return intValue;
                                    }
                                }
                                else if (divide.HasValue && divide > 0)
                                {
                                    if (intValue % divide == remainder)
                                    {
                                        yield return intValue;
                                    }
                                }
                                else yield return intValue;
                            }
                        }
                    }
                }
            }
        }
    }
}

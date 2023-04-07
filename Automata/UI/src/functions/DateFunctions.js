import differenceInDays from 'date-fns/differenceInDays';
import add from 'date-fns/add';
import sub from 'date-fns/sub';
import formatDate from 'date-fns/format';
import startOfDay from 'date-fns/startOfDay';
import startOfHour from 'date-fns/startOfHour';
import startOfMinute from 'date-fns/startOfMinute';
import endOfHour from 'date-fns/endOfHour';
import endOfDay from 'date-fns/endOfDay';
import endOfMinute from 'date-fns/endOfMinute';
import parse from 'date-fns/parse';
import isValid from 'date-fns/isValid';
import isAfter from 'date-fns/isAfter';
import isBefore from 'date-fns/isBefore';
import isEqual from 'date-fns/isEqual';
import set from 'date-fns/set';
import getYear from 'date-fns/getYear';
import hu from 'date-fns/locale/hu';
import en from 'date-fns/locale/en-GB';
import getMonth from 'date-fns/getMonth';
import startOfMonth from 'date-fns/startOfMonth';
import endOfMonth from 'date-fns/endOfMonth';
import isSameMonth from 'date-fns/isSameMonth';
import isSameYear from 'date-fns/isSameYear';
import isSameHour from 'date-fns/isSameHour';
import getHours from 'date-fns/getHours';
import isSameMinute from 'date-fns/isSameMinute';
import getMinutes from 'date-fns/getMinutes';
import isSameDay from 'date-fns/isSameDay';
import getDay from 'date-fns/getDay';
import getDaysInMonth from 'date-fns/getDaysInMonth';
import differenceInMinutes from 'date-fns/differenceInMinutes';
import differenceInHours from 'date-fns/differenceInHours';
import differenceInCalendarDays from 'date-fns/differenceInCalendarDays';
import differenceInMonths from 'date-fns/differenceInMonths';
import differenceInYears from 'date-fns/differenceInYears';
import formatDistanceToNowStrict from 'date-fns/formatDistanceToNowStrict';
import differenceInMilliseconds from 'date-fns/differenceInMilliseconds';
import intervalToDuration from 'date-fns/intervalToDuration';
import { i18n } from '@/plugins/i18n';
import { Languages } from '@/data/languages';

let huModified = {
  ...hu,
  formatDistance: (unit, count) => {
    switch (unit) {
      case 'xYears':
        return `${count} éve`;
      case 'xMonths':
        return `${count} hónapja`;
      case 'xWeeks':
        return `${count} hete`;
      case 'xDays':
        return `${count} napja`;
      case 'xHours':
        return `${count} órája`;
      case 'xMinutes':
        return `${count} perce`;
      case 'xSeconds':
        return 'most';
    }
    return '';
  },
};
let enModified = {
  ...en,
  formatDistance: (unit, count) => {
    switch (unit) {
      case 'xYears':
        return `${count} years ago`;
      case 'xMonths':
        return `${count} months ago`;
      case 'xWeeks':
        return `${count} weeks ago`;
      case 'xDays':
        return `${count} days ago`;
      case 'xHours':
        return `${count} hours ago`;
      case 'xMinutes':
        return `${count} minutes ago`;
      case 'xSeconds':
        return 'now';
    }
    return '';
  },
};

class DateFunctions {
  GetLocale() {
    switch (i18n.locale) {
      case Languages.HU.key:
        return huModified;
      default:
        return enModified;
    }
  }
  DayDiff(date1, date2) {
    if (arguments.length != 2) return;
    return differenceInDays(date1, date2);
  }
  /**
   *
   * @param {Date} date
   * @param {Duration} duration
   */
  Add(date, duration) {
    const resultDate = add(date, duration);
    return resultDate;
  }
  /**
   *
   * @param {Date} date
   * @param {Duration} duration
   */
  Sub(date, duration) {
    const resultDate = sub(date, duration);
    return resultDate;
  }
  /**
   *
   * @param {Date} date
   * @param {DefaultDateFormat} format
   */
  Format(date, format) {
    return formatDate(date, format, { locale: hu });
  }
  StartOfDay(date) {
    return startOfDay(date);
  }
  EndOfDay(date) {
    return endOfDay(date);
  }
  /**
   *
   * @param {Date} date
   * @param {StartOfEndOfOpts} time
   * @returns
   */
  StartOf(date, time) {
    switch (time) {
      case 'minute':
        return startOfMinute(date);
      case 'hour':
        return startOfHour(date);
      case 'day':
        return startOfDay(date);
      case 'month':
        return startOfMonth(date);
      default:
        throw 'Not implemented';
    }
  }
  /**
   *
   * @param {Date} date
   * @param {StartOfEndOfOpts} time
   * @returns
   */
  EndOf(date, time) {
    switch (time) {
      case 'minute':
        return endOfMinute(date);
      case 'hour':
        return endOfHour(date);
      case 'day':
        return endOfDay(date);
      case 'month':
        return endOfMonth(date);
      default:
        throw 'Not implemented';
    }
  }
  /**
   *
   * @param {String} date
   * @param {DefaultDateFormat} format
   * @param {Date} reference
   */
  Parse(date, format, reference) {
    let parsed = parse(date, format, reference ?? new Date());
    return isValid(parsed) ? parsed : null;
  }
  IsSameOrAfter(date1, date2) {
    return isEqual(date1, date2) || isAfter(date1, date2);
  }
  IsSameOrBefore(date1, date2) {
    return isEqual(date1, date2) || isBefore(date1, date2);
  }
  /**
   *
   * @param {Date} date
   * @param {DatePartSet} opt
   * @returns
   */
  Set(date, opt) {
    return set(date, opt);
  }
  /**
   *
   * @param {Date} date
   * @param {DatePartGet} datepart
   * @returns
   */
  Get(date, datepart) {
    switch (datepart) {
      case 'minute':
        return getMinutes(date);
      case 'hour':
        return getHours(date);
      case 'day':
        return getDay(date);
      case 'month':
        return getMonth(date);
      case 'year':
        return getYear(date);
      default:
        throw 'Not implemented';
    }
  }
  IsSameOrBetween(date, minDate, maxDate) {
    return (
      this.IsSameOrAfter(date, minDate) && this.IsSameOrBefore(date, maxDate)
    );
  }
  /**
   *
   * @param {Date} date1
   * @param {Date} date2
   * @param {DatePartGet} datepart
   * @returns
   */
  IsSame(date1, date2, datepart) {
    switch (datepart) {
      case 'minute':
        return isSameMinute(date1, date2);
      case 'hour':
        return isSameHour(date1, date2);
      case 'day':
        return isSameDay(date1, date2);
      case 'month':
        return isSameMonth(date1, date2);
      case 'year':
        return isSameYear(date1, date2);
      default:
        throw 'Not implemented';
    }
  }
  /**
   *
   * @param {Date} d1
   * @param {Date} d2
   */
  IsSameDay(d1, d2) {
    if (!d1 || !d2) {
      return false;
    }
    return (
      d1.getFullYear() === d2.getFullYear() &&
      d1.getMonth() === d2.getMonth() &&
      d1.getDate() === d2.getDate()
    );
  }
  /**
   *
   * @param {Date} date1
   * @param {Date} date2
   * @param {DatePartGet} datepart
   * @returns
   */
  Difference(date1, date2, datepart) {
    switch (datepart) {
      case 'minute':
        return differenceInMinutes(date1, date2);
      case 'hour':
        return differenceInHours(date1, date2);
      case 'day':
        return differenceInCalendarDays(date1, date2);
      case 'month':
        return differenceInMonths(date1, date2);
      case 'year':
        return differenceInYears(date1, date2);
      default:
        throw 'Not implemented';
    }
  }
  GetWeekNumber(d) {
    d = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate()));
    d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay() || 7));
    var yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
    var weekNo = Math.ceil(((d - yearStart) / 86400000 + 1) / 7);
    return weekNo;
  }
  GetDaysInMonth(date) {
    return getDaysInMonth(date);
  }
  IsoStrToShortDate(str) {
    if (!str) {
      return '';
    }
    let d = new Date(str);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    let year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [year, month, day].join('.') + '.';
  }
  IsoStrToDateTime(str) {
    if (!str) {
      return '';
    }
    let d = new Date(str);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    let year = d.getFullYear();
    let hours = '' + d.getHours();
    let minutes = '' + d.getMinutes();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    if (hours.length < 2) hours = '0' + hours;
    if (minutes.length < 2) minutes = '0' + minutes;

    return `${[year, month, day].join('.')}. ${hours}:${minutes}`;
  }

  ToTime(date) {
    return this.Format(date, 'HH:mm');
  }
  ToDateTime(date) {
    return this.Format(date, 'yyyy.MM.dd. HH:mm');
  }
  ToShortDate(date) {
    return this.Format(date, 'yyyy.MM.dd.');
  }
  ToLongDate(date) {
    return this.Format(date, 'yyyy.MM.dd. HH:mm:ss');
  }
  ToServerDate(date) {
    return this.Format(date, 'yyyy-MM-dd');
  }
  ToServerDateTime(date) {
    // prettier-ignore
    return this.Format(date, 'yyyy-MM-dd\'T\'HH:mm:ss');
  }
  ToHumanReadableDistance(date) {
    let result = formatDistanceToNowStrict(date, {
      locale: this.GetLocale(),
      addSuffix: true,
      roundingMethod: 'floor',
    });

    return result;
  }
  GetTimeDifference(start, end) {
    if (!start || !end) {
      return '';
    }

    let diffMs = differenceInMilliseconds(start, end);
    if (diffMs < 0) {
      return '';
    }
    return intervalToDuration({
      start,
      end,
    });
  }
}

let instance = new DateFunctions();
export { instance as DateFunctions };

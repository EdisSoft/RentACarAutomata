import { DateFunctions } from '@/functions/DateFunctions';
import Vue from 'vue';

Vue.filter('toHourMin', function (value) {
  if (!value) return '';
  return DateFunctions.ToTime(new Date(value));
});

Vue.filter('toShortDate', function (value) {
  if (!value) return '';
  return DateFunctions.ToShortDate(new Date(value));
});

Vue.filter('toDateTime', function (value) {
  if (!value) return '';
  return DateFunctions.ToDateTime(new Date(value));
});

Vue.filter('toLongDate', function (value) {
  if (!value) return '';
  return DateFunctions.ToLongDate(new Date(value));
});

Vue.filter('toShortDate', function (value) {
  if (!value) return '';
  return DateFunctions.ToShortDate(new Date(value));
});
var jen = new Set([1]);
var en = new Set([
  4, 5, 7, 9, 10, 11, 12, 14, 15, 17, 19, 21, 22, 24, 25, 27, 29, 31,
]);
var an = new Set([2, 3, 6, 8, 13, 16, 18, 20, 23, 26, 28, 30]);
Vue.filter('toShortDateKeltezes', function (value) {
  if (!value) return '';

  var nap = DateFunctions.Format(new Date(value), 'dd');

  if (jen.has(nap)) {
    return DateFunctions.Format(new Date(value), 'yyyy.MM.dd-jén');
  }
  if (en.has(nap)) {
    return DateFunctions.Format(new Date(value), 'yyyy.MM.dd-én');
  }
  if (an.has(nap)) {
    return DateFunctions.Format(new Date(value), 'yyyy.MM.dd-án');
  }

  return '';
});
Vue.filter('toHumanReadableDate', function (value) {
  if (!value) return '';
  return DateFunctions.ToHumanReadableDistance(new Date(value));
});

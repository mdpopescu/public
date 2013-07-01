// JTable

var JTable = function() {
};

JTable.Setup = function() {
  var table = $('.jtable');
  $('caption', table).addClass('ui-state-default');
  $('th', table).addClass('ui-state-default');
};

// date conversion

function parseJsonDate(jsonDateString) {
  return new Date(parseInt(jsonDateString.substr(6)));
}

// assumes the presence of jQueryUI

function formatDate(date) {
  return $.datepicker.formatDate('mm/dd/yy', date);
}
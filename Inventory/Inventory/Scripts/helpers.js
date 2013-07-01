// JTable

var JTable = function() {
};

JTable.Setup = function() {
  var table = $('.jtable');
  $('caption', table).addClass('ui-state-default');
  $('th', table).addClass('ui-state-default');
  $('td', table).addClass('ui-widget-content');
  $(table).delegate('tr', 'hover', function () {
    $('td', $(this)).toggleClass('ui-state-hover');
  });
  $(table).delegate('tr', 'click', function() {
    $('td', $(this)).toggleClass('ui-state-highlight');
  });
};

JTable.SetupNewRows = function(rows) {
  $(rows).find('td').addClass('ui-widget-content');
};

// date conversion

function parseJsonDate(jsonDateString) {
  return new Date(parseInt(jsonDateString.substr(6)));
}

// assumes the presence of jQueryUI
function formatDate(date) {
  return $.datepicker.formatDate('mm/dd/yy', date);
}
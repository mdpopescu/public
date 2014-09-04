$(function() {
    var masterVm = new ViewModel($.BASE_URL + "Acquisitions/GetAcquisitions", $('#acquisitions')[0]);
    var detailVm = new DetailViewModel($.BASE_URL + "Acquisitions/GetAcquisitionItems", $('#acquisition_items')[0]);

    $('#acquisitions').selectable({
        filter: 'tbody tr',
        selected: function(e, ui) {
            var id = $(ui.selected).data('id');

            $('#acquisitionId').text(id);
            detailVm.update(id);
        }
    });
});

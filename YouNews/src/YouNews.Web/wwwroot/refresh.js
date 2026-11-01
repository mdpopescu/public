(function () {
    var progress = document.querySelector('[data-refresh-progress]');
    if (!progress) {
        return;
    }

    var activity = progress.querySelector('[data-refresh-activity]');
    var bar = progress.querySelector('[data-refresh-bar]');
    var count = progress.querySelector('[data-refresh-count]');

    function show(status) {
        activity.textContent = status.activity;
        if (status.videosTotal > 0) {
            bar.classList.remove('busy');
            bar.style.width = Math.min(100, Math.round(status.videosDone * 100 / status.videosTotal)) + '%';
            count.textContent = status.videosDone + ' of ' + status.videosTotal + ' videos';
        } else {
            bar.classList.add('busy');
            bar.style.width = '100%';
            count.textContent = '';
        }
    }

    function poll() {
        fetch('/status', { cache: 'no-store' })
            .then(function (response) { return response.json(); })
            .then(function (status) {
                if (!status.running) {
                    window.location.reload();
                    return;
                }
                show(status);
                window.setTimeout(poll, 2000);
            })
            .catch(function () { window.setTimeout(poll, 5000); });
    }

    window.setTimeout(poll, 1500);
})();
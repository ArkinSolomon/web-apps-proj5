let expectClose = false;
function toggleModal(shouldExpectClose = true) {
    const $body = $(document.body);
    if ($body.hasClass('plan-modify-open')) {
        $body.removeClass('plan-modify-open');
        expectClose = shouldExpectClose;
        document.getElementById('plan-modify-window').close();
    } else {
        $body.addClass('plan-modify-open');
        document.getElementById('plan-modify-window').showModal();
    }
}

$(document).ready(function () {
    $('#plan-modify-window').on('close', () => {
        if (expectClose) {
            expectClose = false;
            return;
        }

        toggleModal(false);
    });

    $('#plan-modify-close').click(() => {
        expectClose = true;
        toggleModal();
    });
});

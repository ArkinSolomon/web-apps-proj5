$(document).ready(() => {
    const $nameInput = $('#name-input');
    const $emailInput = $('#email-input');
    const $passwordInput = $('#password-input');
    const $submit = $('#submit-button');

    if ($nameInput.length) {
        $nameInput.keyup(changed);
    }

    $emailInput.keyup(changed);
    $passwordInput.keyup(changed);

    function changed() {
        let nameIsValid = true;
        if ($nameInput.length) {
            const nameValue = $nameInput.val();
            nameIsValid = nameValue.trim() === nameValue && nameValue.length >= 3 && nameValue.length <= 32 && /^[a-z\s]+$/i.test(nameValue);
            if (!nameIsValid) {
                $nameInput.addClass('error');
            } else {
                $nameInput.removeClass('error');
            }
        }

        const emailInput = $emailInput.val();
        const emailIsValid = emailInput.trim() === emailInput && emailInput.length > 3 && emailInput.length <= 32 && /^[a-z][a-z0-9]+@[a-z][a-z0-9]+\.[a-z]{2,}$/i.test(emailInput);
        if (!emailIsValid) {
            $emailInput.addClass('error');
        } else {
            $emailInput.removeClass('error');
        }

        const passwordInput = $passwordInput.val();
        const passwordIsValid = passwordInput.length >= 6 && passwordInput.length <= 16;
        if (!passwordIsValid) {
            $passwordInput.addClass('error');
        } else {
            $passwordInput.removeClass('error');
        }

        $submit.attr('disabled', !nameIsValid || !emailIsValid || !passwordIsValid);
    }
    changed();
});
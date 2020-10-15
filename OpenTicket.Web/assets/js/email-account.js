(function () {
    var protocolOptions = {
        Imap: 1,
        Pop3: 2,
        M365: 3
    };
    function tokenReceiveCallback(event) {
        if (event.data.AccessToken && !/^\s*$/.test(event.data.AccessToken)) {
            let submitButton = document.querySelector('button[type="submit"]');
            submitButton.removeAttribute('disabled');
            let form = document.getElementsByTagName('form')[0];
            form.setAttribute('action', form.getAttribute('data-original-action'));
            form.removeAttribute('data-original-action');
            form.removeAttribute('target');
        }
    }

    function signInExternal() {
        let form = document.getElementsByTagName('form')[0];
        form.setAttribute('target', '_blank');
        form.setAttribute('data-original-action', form.getAttribute('action'));
        form.setAttribute('action', form.getAttribute('data-external-login'));
        let submitButton = document.querySelector('button[type="submit"]');
        submitButton.setAttribute('disabled', 'disabled');
    }

    function setVisibility(className, isVisible) {
        var elements = document.getElementsByClassName(className);
        var toggle = isVisible
            ? function (element) {
                element.style.removeProperty('display');
            }
            : function (element) {
                element.style.display = 'none';
            };
        Array.prototype.slice.call(elements).forEach(toggle);
    }

    function getProtocol() {
        return parseInt(document.getElementById('Protocol').value);
    }

    function toggleDisplay() {
        var protocol = getProtocol();
        var isLocalAccount = protocol !== protocolOptions.M365;
        var submitButton = document.getElementById('btn-submit');
        if (isLocalAccount) {
            setVisibility('local-only', true);
            setVisibility('remote-only', false);
            submitButton.removeAttribute('disabled');
        } else {
            setVisibility('local-only', false);
            setVisibility('remote-only', true);
            submitButton.setAttribute('disabled', 'disabled');
        }
        setVisibility('imap-only', protocol === protocolOptions.Imap);
    }

    function populateExternalAccounts() {
        var selectedProtocol = getProtocol();
        if (selectedProtocol !== protocolOptions.M365) return;
        
        var externalAccountDropdown = document.getElementById('ExternalAccountId');
        for (var i = externalAccountDropdown.options.length - 1; i >= 0; i--)
            externalAccountDropdown.options[i].remove();
        fetch('/ExternalAccount', {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => response.json())
            .then(data => data.filter(d => d.Protocol === selectedProtocol))
            .then(externalAccounts => {
                if (externalAccounts === undefined) return;
                for (var i = 0; i < externalAccounts.length; i++) {
                    var ea = externalAccounts[i];
                    var option = document.createElement('option');
                    option.setAttribute('value', ea.Id);
                    option.textContent = `${ea.Name} (${ea.Identifier})`;
                    externalAccountDropdown.options.add(option);
                }
            });
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.getElementById('Protocol').addEventListener('change', toggleDisplay);
        document.getElementById('Protocol').addEventListener('change', populateExternalAccounts);
        document.getElementById('btn-login').addEventListener('click', signInExternal);
    });
    window.addEventListener('message', tokenReceiveCallback);
    toggleDisplay();
    populateExternalAccounts();
})();

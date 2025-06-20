document.addEventListener('DOMContentLoaded', function () {
    const detailsContainer = document.getElementById('voucher-details-container');
    const addDetailButton = document.getElementById('add-detail-line');

    // This variable 'accountOptions' is defined in a <script> tag in Create.cshtml
    // and holds the HTML <option> elements for the account dropdown.

    function getDetailIndex() {
        return detailsContainer.children.length;
    }

    addDetailButton.addEventListener('click', function () {
        let detailIndex = getDetailIndex();
        const newDetailRowHtml = `
            <div class="row voucher-detail-row mb-2">
                <div class="col-md-5">
                    <select name="Voucher.Details[${detailIndex}].AccountId" class="form-control account-dropdown" required>
                        <option value="">-- Select Account --</option>
                        ${accountOptions}
                    </select>
                </div>
                <div class="col-md-3">
                    <input type="number" name="Voucher.Details[${detailIndex}].DebitAmount" class="form-control debit-input" value="0.00" step="0.01" />
                </div>
                <div class="col-md-3">
                    <input type="number" name="Voucher.Details[${detailIndex}].CreditAmount" class="form-control credit-input" value="0.00" step="0.01" />
                </div>
                <div class="col-md-1">
                    <button type="button" class="btn btn-danger btn-sm remove-detail-line">×</button>
                </div>
            </div>`;

        detailsContainer.insertAdjacentHTML('beforeend', newDetailRowHtml);
        updateEventListeners();
    });

    function updateEventListeners() {
        document.querySelectorAll('.remove-detail-line').forEach(button => {
            button.removeEventListener('click', removeRow);
            button.addEventListener('click', removeRow);
        });

        document.querySelectorAll('.debit-input, .credit-input').forEach(input => {
            input.removeEventListener('input', calculateTotals);
            input.addEventListener('input', calculateTotals);
        });
    }

    function removeRow(event) {
        event.target.closest('.voucher-detail-row').remove();
        // After removing a row, we must re-index the remaining rows so model binding doesn't break.
        reindexRows();
        calculateTotals();
    }

    function reindexRows() {
        const rows = detailsContainer.querySelectorAll('.voucher-detail-row');
        rows.forEach((row, index) => {
            row.querySelectorAll('select, input').forEach(input => {
                if (input.name) {
                    input.name = input.name.replace(/\[\d+\]/, `[${index}]`);
                }
            });
        });
    }

    function calculateTotals() {
        let totalDebit = 0;
        let totalCredit = 0;

        document.querySelectorAll('.voucher-detail-row').forEach(row => {
            const debit = parseFloat(row.querySelector('.debit-input').value) || 0;
            const credit = parseFloat(row.querySelector('.credit-input').value) || 0;
            totalDebit += debit;
            totalCredit += credit;
        });

        document.getElementById('total-debit').textContent = totalDebit.toFixed(2);
        document.getElementById('total-credit').textContent = totalCredit.toFixed(2);

        const balance = totalDebit - totalCredit;
        const balanceElement = document.getElementById('balance');
        const balanceContainer = document.getElementById('balance-container');
        balanceElement.textContent = Math.abs(balance).toFixed(2);

        if (balance.toFixed(2) !== '0.00') {
            balanceContainer.classList.add('text-danger');
            balanceContainer.classList.remove('text-success');
        } else {
            balanceContainer.classList.remove('text-danger');
            balanceContainer.classList.add('text-success');
        }
    }

    // Initial setup for any pre-rendered rows
    updateEventListeners();
    calculateTotals();
});
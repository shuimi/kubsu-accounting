using accounting.Services;
using System.Collections.Generic;
using System.Data;

namespace accounting
{
    public partial class EnterpriseAccountingMainForm : Form
    {
        ApplicationContext applicationContext;

        WorkersService workersService;
        PositionsService positionsService;
        AccountsService accountsService;
        AdvanceReportsService advanceReportsService;
        AuditService auditService;


        public EnterpriseAccountingMainForm()
        {
            InitializeComponent();

            applicationContext = new ApplicationContext();

            workersService = new WorkersService(applicationContext);
            positionsService = new PositionsService(applicationContext);
            accountsService = new AccountsService(applicationContext);
            advanceReportsService = new AdvanceReportsService(applicationContext);
            auditService = new AuditService();

            statusInput.Items.Add("Нанят");
            statusInput.Items.Add("Уволен");

            updatePositionsList();
            updateWorkersList();
            updateAccountsList();
            updateAdvanceReportsList();
            auditAccountsListUpdate();
            updateAdvanceReportInputsCategories();
            updateAuditDateOptionsAndWorkers();

            saveFileDialog.Filter = "Excel files (*.xls or .xlsx)|.xls;*.xlsx";
        }


        private void updatePositionsList()
        {
            positionsList.Items.Clear();
            workersSortType.Items.Clear();

            var positions = positionsService.getPositionsList();

            if (positions != null)
            {
                var positionNames = positions.Select(position => position.Name).ToArray();

                positionsList.Items.AddRange(positionNames);

                workersSortType.Items.Add("Все");
                workersSortType.SelectedItem = workersSortType.Items[0];
                workersSortType.Items.AddRange(positionNames);
            }
            updateAdvanceReportInputsCategories();
        }

        private void dialogEmptyName()
        {
            string message = "Имя не может быть пустым";
            string caption = "Неверный ввод";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
        }

        private void dialogEmptyElement()
        {
            string message = "Не выбран элемент";
            string caption = "Неверный ввод";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
        }

        private void dialogIncorrectInput()
        {
            string message = "Введённые данные имеют неверный формат";
            string caption = "Неверный ввод";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
        }

        private void dialogEmptyQueryResult()
        {
            string message = "Элемент не найден в базе";
            string caption = "Элемент не существует";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);
        }

        private void updatePositionInputs(string name, string? description)
        {
            positionDescriptionInput.Text = description;
            positionNameInput.Text = name;
        }

        private void addPositionButtonClick(object sender, EventArgs e)
        {
            var selectedItemName = positionsList.SelectedItem;

            if (selectedItemName == null)
            {
                positionsService.addPosition(
                        new Position
                        {
                            Name = "Новая должность " + positionsService.getPositionsList().Count().ToString(),
                            Description = "Пустое описание",
                        }
                    );
                updatePositionsList();
            }
            else
            {
                if (positionsService.isExist(new Position
                {
                    Name = positionNameInput.Text,
                    Description = positionDescriptionInput.Text,
                }))
                {
                    string message = "Невозможно добавить существующую позицию, введите уникальное название";
                    string caption = "Неверный ввод";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);

                    return;
                }
                if (positionNameInput.Text.Length != 0)
                {
                    positionsService.addPosition(
                        new Position
                        {
                            Name = positionNameInput.Text,
                            Description = positionDescriptionInput.Text,
                        }
                    );
                    updatePositionsList();
                    updatePositionInputs("", "");
                }
                else
                {
                    dialogEmptyName();
                }
            }
        }

        private void deletePositionButtonClick(object sender, EventArgs e)
        {
            var selectedItemName = positionsList.SelectedItem;

            if (selectedItemName != null)
            {
                var index = positionsService.findIndexByName(selectedItemName.ToString());

                string message = "Удалить?";
                string caption = "Подтвердите";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
                if (result == DialogResult.Yes)
                {
                    positionsService.deletePosition(index);
                    updatePositionInputs("", "");
                }
            }
            else
            {
                dialogEmptyElement();
            }
            updatePositionsList();
        }

        private void updatePositionButtonClick(object sender, EventArgs e)
        {

            var selectedItemName = positionsList.SelectedItem;

            if (selectedItemName != null)
            {
                var name = positionNameInput.Text;
                var description = positionDescriptionInput.Text;

                if (name != null)
                {
                    var id = positionsService.findIndexByName(selectedItemName.ToString());

                    positionsService
                        .updatePosition(id, new Position { Name = name, Description = description });

                    updatePositionsList();
                    updatePositionInputs("", "");
                }
                else
                {
                    dialogEmptyName();
                }
            }
            else
            {
                dialogEmptyElement();
            }
        }

        private void positionsListSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItemName = positionsList.SelectedItem;

            if (selectedItemName != null)
            {
                var id = positionsService.findIndexByName(selectedItemName.ToString());
                var position = positionsService.getPositionById(id);

                updatePositionInputs(position.Name, position.Description);
            }
            else
            {
                updatePositionInputs("", "");
            }
        }



        List<Worker> workers = new List<Worker>();

        private void reloadWorkers()
        {
            workers = workersService.getWorkersList();
        }

        private void updateWorkersInputs(Worker worker)
        {
            if (worker != null)
            {
                firstNameInput.Text = worker.Firstname;
                lastNameInput.Text = worker.Lastname;
                middleNameInput.Text = worker.Middlename;

                contractInput.Text = worker.ContractId;
                dateInput.Text = worker.EmploymentDate;
                statusInput.Text = worker.Status;

                if (worker.Positions != null)
                {
                    var positions = positionsService.getPositionsList();
                    var positionNames = worker.Positions.Select(position => position.Name).ToArray();

                    for (int i = 0; i < positions.Count(); i++)
                    {
                        var contains = positionNames.Contains(workerPositions.Items[i]);
                        workerPositions.SetItemChecked(i, contains);
                    }
                }
            }
            else
            {
                firstNameInput.Text = "";
                lastNameInput.Text = "";
                middleNameInput.Text = "";

                contractInput.Text = "";
                dateInput.Text = "";
                statusInput.Text = "";

                workerPositions.Items.Clear();
                var positions = positionsService.getPositionsList();
                var positionNames = positions.Select(position => position.Name).ToArray();
                workerPositions.Items.AddRange(positionNames);
            }
        }

        private void updateWorkersList()
        {
            reloadWorkers();

            var selectedItem = workersSortType.SelectedItem;

            workersList.Items.Clear();
            updateWorkersInputs(null);

            if (selectedItem.Equals("Все"))
            {
                workersList.Items.AddRange(
                    workers.Select(
                        w => $"{w.Id}. {w.Lastname} {w.Firstname.First()} {w.Middlename.First()} - {w.ContractId} - {w.Status}"
                    ).ToArray()
                );
            }
            else
            {
                workersList.Items.AddRange(
                    workers
                    .Where(
                        worker => worker
                            .Positions
                            .Where(
                                position => position.Name == selectedItem.ToString()
                            )
                            .Count() != 0
                    )
                    .Select(
                        w => $"{w.Id}. {w.Lastname} {w.Firstname.First()} {w.Middlename.First()} - {w.ContractId} - {w.Status}"
                    ).ToArray()
                );
            }
            updateAdvanceReportInputsCategories();
            updateAuditDateOptionsAndWorkers();
        }

        private void workersSortTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            updateWorkersList();
        }

        private int getId(string selectedItem)
        {
            return int.Parse(selectedItem.Split('.')[0]);
        }

        private void workersListSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = workersList.SelectedItem;

            if (selectedItem != null)
            {
                var id = getId(selectedItem.ToString());
                var worker = workersService.getWorkerById(id);
                updateWorkersInputs(worker);
            }
            else
            {
                updateWorkersInputs(null);
            }
        }

        private void addWorkerButton_Click(object sender, EventArgs e)
        {

            var selectedItem = workersList.SelectedItem;
            var selectedIndex = workersList.SelectedIndex;

            workersService.addWorker(new Worker
            {
                Firstname = "Иванов",
                Lastname = "Иван",
                Middlename = "Иванович",
                ContractId = "-1",
                EmploymentDate = "01.01.1980",
                Status = "Уволен",
            });

            updateWorkersList();
        }

        private void updateWorkerButton_Click(object sender, EventArgs e)
        {
            var selectedItem = workersList.SelectedItem;
            var selectedIndex = workersList.SelectedIndex;

            if (selectedItem != null)
            {
                var id = getId(selectedItem.ToString());
                var positions = positionsService.getPositionsList()
                    .Where(position => workerPositions.CheckedItems.Contains(position.Name))
                    .ToList();

                workersService.updateWorker(id, new Worker {
                    Firstname = firstNameInput.Text,
                    Lastname = lastNameInput.Text,
                    Middlename = middleNameInput.Text,
                    ContractId = contractInput.Text,
                    EmploymentDate = dateInput.Text,
                    Status = statusInput.Text,
                    Positions = positions,
                });
            }
            else
            {
                dialogEmptyElement();
            }

            updateWorkersList();
        }

        private void deleteWorkerButton_Click(object sender, EventArgs e)
        {
            var selectedItem = workersList.SelectedItem;
            var selectedIndex = workersList.SelectedIndex;

            if (selectedItem != null)
            {
                var id = getId(selectedItem.ToString());
                workersService.deleteWorker(id);
            }
            else
            {
                dialogEmptyElement();
            }

            updateWorkersList();
        }

        private void workerPositions_SelectedIndexChanged(object sender, EventArgs e) { }



        private void updateAccountsList()
        {
            accountsList.Items.Clear();

            var accounts = accountsService.getAccountsList();

            if (accounts != null)
            {
                var accountNames = accounts.Select(account => account.Name).ToArray();

                accountsList.Items.AddRange(accountNames);
            }
            updateAdvanceReportInputsCategories();
        }

        private void updateAccountsInputs(string name, string? description, int id)
        {
            accountDescriptionInput.Text = description;
            accountNameInput.Text = name;
            accountNumber.Text = id.ToString();
        }

        private void addAccount_Click(object sender, EventArgs e)
        {
            var selectedItemName = accountsList.SelectedItem;

            if (selectedItemName == null)
            {
                accountsService.addAccount(
                        new Account
                        {
                            Name = "Новый счёт " + accountsService.getAccountsList().Count().ToString(),
                            Description = "Пустое описание",
                        }
                    );
                updateAccountsList();
            }
            else
            {
                if (accountsService.isExist(new Account
                {
                    Name = accountNameInput.Text,
                    Description = accountDescriptionInput.Text,
                }))
                {
                    string message = "Невозможно добавить существующий счёт, введите уникальное название";
                    string caption = "Неверный ввод";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    DialogResult result;

                    result = MessageBox.Show(message, caption, buttons);

                    return;
                }
                if (accountNameInput.Text.Length != 0)
                {
                    accountsService.addAccount(
                        new Account
                        {
                            Name = accountNameInput.Text,
                            Description = accountDescriptionInput.Text,
                        }
                    );
                    updateAccountsList();
                    updateAccountsInputs("", "", -1);
                }
                else
                {
                    dialogEmptyName();
                }
            }
            auditAccountsListUpdate();
        }

        private void deleteAccount_Click(object sender, EventArgs e)
        {
            var selectedItem = accountsList.SelectedItem;
            var selectedIndex = accountsList.SelectedIndex;

            if (selectedItem != null)
            {
                var index = accountsService.findIndexByName(selectedItem.ToString());

                string message = "Удалить?";
                string caption = "Подтвердите";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                result = MessageBox.Show(message, caption, buttons);
                if (result == DialogResult.Yes)
                {
                    accountsService.deleteAccount(index);
                    updateAccountsInputs("", "", -1);
                }
            }
            else
            {
                dialogEmptyElement();
            }
            updateAccountsList();
            auditAccountsListUpdate();
        }

        private void updateAccount_Click(object sender, EventArgs e)
        {
            var selectedItemName = accountsList.SelectedItem;

            if (selectedItemName != null)
            {
                var name = accountNameInput.Text;
                var description = accountDescriptionInput.Text;

                if (name != null)
                {
                    var id = accountsService.findIndexByName(selectedItemName.ToString());

                    accountsService
                        .updateAccount(id, new Account { Name = name, Description = description });

                    updateAccountsList();
                    updateAccountsInputs("", "", -1);
                }
                else
                {
                    dialogEmptyName();
                }
            }
            else
            {
                dialogEmptyElement();
            }
            auditAccountsListUpdate();
        }

        private void accountsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItemName = accountsList.SelectedItem;

            if (selectedItemName != null)
            {
                var id = accountsService.findIndexByName(selectedItemName.ToString());
                var account = accountsService.getAccountById(id);

                updateAccountsInputs(account.Name, account.Description, account.Id);
            }
            else
            {
                updateAccountsInputs("", "", -1);
            }
        }

        
        List<AdvanceReport> advanceReports = new List<AdvanceReport>();

        private void updateAdvanceReportsList()
        {
            advanceReportsList.Items.Clear();

            var reports = advanceReportsService.getAdvanceReportsList();

            if (reports != null)
            {
                advanceReports = reports.ToList();

                var reportsNames = advanceReports
                    .Select(account => 
                        $"{account.Id}. " +
                        $"{account.Appointment} - " +
                        $"{account.AccountablePerson?.Lastname} " +
                        $"{account.AccountablePerson?.Firstname.First()} " +
                        $"{account.AccountablePerson?.Middlename.First()}"
                    ).ToArray();

                advanceReportsList.Items.Clear();
                advanceReportsList.Items.AddRange(reportsNames);
            }
            updateAdvanceReportInputsCategories();
            updateAuditDateOptionsAndWorkers();
        }


        List<Worker> heads = new List<Worker>();
        List<Worker> accountable = new List<Worker>();
        List<Worker> chiefAccountants = new List<Worker>();
        List<Worker> accountants = new List<Worker>();

        private string getFullname(Worker worker) 
        {
            return $"{worker.Id}. {worker.Lastname} {worker.Firstname.First()} {worker.Middlename.First()}";    
        }

        private string[] getWorkersNames(List<Worker> workers)
        {
            return workers.Select(worker => getFullname(worker)).ToArray();
        }

        private void updateAdvanceReportInputsCategories()
        {
            heads.Clear();
            accountable.Clear();
            chiefAccountants.Clear();
            accountants.Clear();

            debitAccountInput.Items.Clear();
            creditAccountInput.Items.Clear();
            headInput.Items.Clear();
            accountablePersonInput.Items.Clear();
            chiefAccountantInput.Items.Clear();
            accountantInput.Items.Clear();

            var accounts = accountsService.getAccountsList();

            if (accounts != null)
            {
                var accountNames = accounts.Select(a => $"{a.Id}. {a.Name} {a.Description}").ToArray();
                debitAccountInput.Items.AddRange(accountNames);
                creditAccountInput.Items.AddRange(accountNames);
            }

            var workers = workersService.getWorkersList();

            if (workers != null)
            {
                heads = workers.Where(worker => 
                    worker.Positions
                        .Select(position => position.Name)
                        .Contains("Директор")
                ).ToList();

                accountable = workers.Where(worker =>
                    worker.Positions
                        .Select(position => position.Name)
                        .Contains("Исполнитель")
                ).ToList();

                chiefAccountants = workers.Where(worker =>
                    worker.Positions
                        .Select(position => position.Name)
                        .Contains("Главный бухгалтер")
                ).ToList();

                accountants = workers.Where(worker =>
                    worker.Positions
                        .Select(position => position.Name)
                        .Contains("Бухгалтер")
                ).ToList();
                
                headInput.Items.AddRange(getWorkersNames(heads));
                accountablePersonInput.Items.AddRange(getWorkersNames(accountable));

                chiefAccountantInput.Items.AddRange(getWorkersNames(chiefAccountants));
                accountantInput.Items.AddRange(getWorkersNames(accountants));
            }
        }

        private string getWorkerInitialsWithId(Worker? worker)
        {
            if (worker == null) return "";
            return $"{worker?.Id}. {worker?.Lastname} {worker?.Firstname.First()} {worker?.Middlename.First()}";
        }

        private void updateAdvanceReportInputs(AdvanceReport? report)
        {
            if (report == null)
            {
                advanceReportDateInput.Text = "";
                advanceReportSum.Text = "";
                advanceReportNumber.Text = "";

                headInput.Text = "";
                accountablePersonInput.Text = "";
                chiefAccountantInput.Text = "";
                accountantInput.Text = "";

                advanceAppointment.Text = "";

                totalRecieved.Text = "0.0";
                usedUp.Text = "0.0";

                debitAccountInput.Text = "";
                debitValue.Text = "";
                creditAccountInput.Text = "";
                creditValue.Text = "";

                accountingRecordsList.Items.Clear();
                return;
            }

            advanceReportDateInput.Text = report.Date;
            advanceReportSum.Text = report.getTotal().ToString();
            advanceReportNumber.Text = report.Id.ToString();
            
            headInput.Text = getWorkerInitialsWithId(report.Head);
            accountablePersonInput.Text = getWorkerInitialsWithId(report.AccountablePerson);
            chiefAccountantInput.Text = getWorkerInitialsWithId(report.ChiefAccountant);
            accountantInput.Text = getWorkerInitialsWithId(report.Accountant);
            
            advanceAppointment.Text = report.Appointment;

            totalRecieved.Text = report.getRecieved().ToString();
            usedUp.Text = report.getSpent().ToString();

            var records = report.AccountingRecords?
                .Select(
                    record => $"{record.Id}. " +
                    $"Дебет: {record.Debit}, " +
                    $"{getAccountNameWithId(record.DebitAccount)};" +
                    $"Кредит: {record.Credit}, " +
                    $"{getAccountNameWithId(record.DebitAccount)}"
                )
                .ToArray();

            accountingRecordsList.Items.Clear();
            accountingRecordsList.Items.AddRange(records);
        }

        private string getAccountNameWithId(Account? account)
        {
            if (account == null) return "";
            return $"{account?.Id}. {account?.Name}";
        }

        private void updateAccountingRecordsInputs(AccountingRecord record)
        {
            debitAccountInput.Text = getAccountNameWithId(record.DebitAccount);
            debitValue.Text = record.Debit.ToString();
            creditAccountInput.Text = getAccountNameWithId(record.CreditAccount);
            creditValue.Text = record.Credit.ToString();
        }

        private void addAdvanceReport_Click(object sender, EventArgs e)
        {
            advanceReportsService.addAdvanceReport(new AdvanceReport { 
                Date="01.01.1980",
                Appointment="Назначение аванса",
            });
            updateAdvanceReportsList();
        }

        private void deleteAdvanceReport_Click(object sender, EventArgs e)
        {
            var selectedItem = advanceReportsList.SelectedItem;

            if (selectedItem != null)
            {
                string message = "Удалить?";
                string caption = "Подтвердите";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                
                result = MessageBox.Show(message, caption, buttons);
                
                var selectedIndex = getId(selectedItem.ToString());

                if (result == DialogResult.Yes)
                {
                    advanceReportsService.deleteAdvanceReport(selectedIndex);
                    updateAccountsInputs("", "", -1);
                }
            }
            else
            {
                dialogEmptyElement();
            }
            updateAdvanceReportsList();
        }

        private void advanceReportsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateSelectedAdvanceReport();
        }

        private void updateSelectedAdvanceReport()
        {
            var selectedItem = advanceReportsList.SelectedItem;

            if (selectedItem != null)
            {
                var id = getId(selectedItem.ToString());
                var report = advanceReportsService.getAdvanceReportById(id);

                updateAdvanceReportInputs(report);
            }
        }

        private void accountingRecordsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = accountingRecordsList.SelectedItem;

            if (selectedItem != null)
            {
                var id = getId(selectedItem.ToString());
                var record = advanceReportsService.getAccountingRecordById(id);
                updateAccountingRecordsInputs(record);
            }
        }

        private void addAccountingRecord_Click(object sender, EventArgs e)
        {
            var selectedReportItem = advanceReportsList.SelectedItem;
            
            if (selectedReportItem != null)
            {
                var id = getId(selectedReportItem.ToString());
                var report = advanceReportsService.getAdvanceReportById(id);

                if (report == null)
                {
                    dialogEmptyQueryResult();
                    return;
                }

                if (report.AccountingRecords == null)
                {
                    report.AccountingRecords = new List<AccountingRecord>();
                }

                report.AccountingRecords.Add(new AccountingRecord {
                    Credit = 0.0,
                    Debit = 0.0,
                });

                advanceReportsService.updateAdvanceReport(id, report);
            }
            else
            {
                dialogEmptyElement();
            }
            updateSelectedAdvanceReport();
        }

        private void deleteAccountingRecord_Click(object sender, EventArgs e)
        {
            var selectedReportItem = advanceReportsList.SelectedItem;

            if (selectedReportItem != null)
            {
                var id = getId(selectedReportItem.ToString());
                var report = advanceReportsService.getAdvanceReportById(id);

                if (report == null)
                {
                    dialogEmptyQueryResult();
                    return;
                }

                if (report.AccountingRecords == null)
                {
                    report.AccountingRecords = new List<AccountingRecord>();
                }

                var selectedItem = accountingRecordsList.SelectedItem;

                if (selectedItem != null)
                {
                    var recordId = getId(selectedItem.ToString());

                    report.AccountingRecords.RemoveAll(r => r.Id == recordId);
                    advanceReportsService.updateAdvanceReport(id, report);
                }
            }
            else
            {
                dialogEmptyElement();
            }
            updateSelectedAdvanceReport();
        }

        private void updateAccountingRecord_Click(object sender, EventArgs e)
        {
            var selectedReportItem = advanceReportsList.SelectedItem;

            if (selectedReportItem != null)
            {
                var id = getId(selectedReportItem.ToString());
                var report = advanceReportsService.getAdvanceReportById(id);

                if (report == null)
                {
                    dialogEmptyQueryResult();
                    return;
                }

                if (report.AccountingRecords == null)
                {
                    report.AccountingRecords = new List<AccountingRecord>();
                }

                var selectedItem = accountingRecordsList.SelectedItem;

                if (selectedItem != null)
                {
                    var recordId = getId(selectedItem.ToString());

                    var record = report.AccountingRecords.FirstOrDefault(r => r.Id == recordId);

                    var debitName = debitAccountInput.Text.ToString();
                    var creditName = creditAccountInput.Text.ToString();
                    
                    if (debitName.Equals("") || creditName.Equals(""))
                    {
                        dialogIncorrectInput();
                        return;
                    }

                    var debitAccountId = getId(debitName);
                    var creditAccountId = getId(creditName);

                    var debitAccount = accountsService.getAccountById(debitAccountId);
                    var creditAccount = accountsService.getAccountById(creditAccountId);

                    if (record != null)
                    {
                        try { 
                            record.Debit = double.Parse(debitValue.Text);
                            record.Credit = double.Parse(creditValue.Text);
                        }
                        catch (Exception)
                        {
                            dialogIncorrectInput();
                        }
                        record.DebitAccount = debitAccount;
                        record.CreditAccount = creditAccount;
                    }

                    advanceReportsService.updateAdvanceReport(id, report);
                }
            }
            else
            {
                dialogEmptyElement();
            }
            updateSelectedAdvanceReport();
        }

        private void updateAdvanceReport_Click(object sender, EventArgs e)
        {
            var selectedReportItem = advanceReportsList.SelectedItem;

            if (selectedReportItem != null)
            {
                var id = getId(selectedReportItem.ToString());
                var report = advanceReportsService.getAdvanceReportById(id);

                if (report == null)
                {
                    dialogEmptyQueryResult();
                    return;
                }

                if (report.AccountingRecords == null)
                {
                    report.AccountingRecords = new List<AccountingRecord>();
                }

                var headRecord = headInput.Text.ToString();
                var accountablePersonRecord = accountablePersonInput.Text.ToString();
                var chiefAccountantRecord = chiefAccountantInput.Text.ToString();
                var accountantRecord = accountantInput.Text.ToString();

                if (!headRecord.Equals(""))
                {
                    var head = workersService.getWorkerById(getId(headRecord));
                    report.Head = head;
                }

                if (!accountablePersonRecord.Equals(""))
                {
                    var accountablePerson = workersService.getWorkerById(getId(accountablePersonRecord));
                    report.AccountablePerson = accountablePerson;
                }

                if (!chiefAccountantRecord.Equals(""))
                {
                    var chiefAccountant = workersService.getWorkerById(getId(chiefAccountantRecord));
                    report.ChiefAccountant = chiefAccountant;
                }
                
                if (!accountantRecord.Equals(""))
                {
                    var accountant = workersService.getWorkerById(getId(accountantRecord));
                    report.Accountant = accountant;
                }

                report.Date = advanceReportDateInput.Text;
                report.Appointment = advanceAppointment.Text;

                advanceReportsService.updateAdvanceReport(id, report);
            }
            else
            {
                dialogEmptyElement();
            }
            updateAdvanceReportsList();
        }

        private void auditAccountsListUpdate()
        {
            var accounts = accountsService.getAccountsList();
            
            if (accounts != null)
            {
                var accountsRecords = accounts
                    .Select(a => $"{a.Id}. {a.Name} {a.Description}")
                    .ToArray();

                auditAccountsList.Items.Clear();
                auditAccountsList.Items.AddRange(accountsRecords);
            }
        }

        private void auditAccountsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            auditAccountingRecordsList.Items.Clear();

            var checkedItems = auditAccountsList.CheckedItems;

            if (checkedItems != null)
            {
                List<Account> accountsRecords = new List<Account>();

                foreach (string item in checkedItems)
                {
                    var account = accountsService.getAccountById(getId(item));
                    accountsRecords.Add(account);
                }

                var accountingRecords = advanceReportsService.getAccountingRecordsList();

                if (accountingRecords != null)
                {
                    var filteredAccountingRecords = accountingRecords
                        .Where(r =>
                            accountsRecords.Contains(r.DebitAccount) ||
                            accountsRecords.Contains(r.CreditAccount)
                        )
                        .Select(r => $"{r.Id}. Дебет: {r.Debit}; Кредит: {r.Credit};")
                        .ToArray();

                    auditAccountingRecordsList.Items.AddRange(filteredAccountingRecords);

                }

            }
        }

        private void updateAuditDateOptionsAndWorkers()
        {
            var workers = workersService.getWorkersList()
                .Select(worker => getWorkerInitialsWithId(worker))
                .ToArray();
            var reports = advanceReportsService.getAdvanceReportsList();
            var dates = reports
                .Select(report => report.Date.Substring(2).TrimStart())
                .Distinct()
                .ToArray();
            
            auditWorkersList.Items.Clear();
            auditWorkersList.Items.AddRange(workers);

            auditAllWorkersPeriodInput.Items.Clear();
            auditAllWorkersPeriodInput.Items.AddRange(dates);
            auditSelectedWorkersPeriodInput.Items.Clear();
            auditSelectedWorkersPeriodInput.Items.AddRange(dates);
        }

        private void buttonWorkersReport_Click(object sender, EventArgs e)
        {
            var period = auditAllWorkersPeriodInput.SelectedItem;

            if (period != null)
            {
                var reports = advanceReportsService.getAdvanceReportsList();

                DataTable data = new DataTable();

                data.Columns.Add("Работник");
                data.Columns.Add("Остаток");
                data.Columns.Add("Оборот");

                var auditReport = reports
                    .Where(report => report.Date.Contains(period.ToString()))
                    .GroupBy(report => report.AccountablePerson)
                    .Select(group => new List<string> {
                            getWorkerInitialsWithId(group.Key),
                            group.Aggregate(0.0, (sum, report) => sum + (report.getRecieved() - report.getSpent()))
                                .ToString(),
                            group.Aggregate(0.0, (sum, report) => sum + report.getTotal())
                                .ToString(),
                        }
                        .ToArray()
                    )
                    .ToList();
                
                foreach (var row in auditReport)
                {
                    data.Rows.Add(row);
                }

                tableView.DataSource = data;
                dialogSaveFile(data);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonMoneyReport_Click(object sender, EventArgs e)
        {
            var period = auditSelectedWorkersPeriodInput.SelectedItem;
            var selectedWorkers = auditWorkersList.CheckedItems;

            if (period != null && selectedWorkers != null && selectedWorkers.Count != 0)
            {
                List<Worker> checkedWorkers = new List<Worker>();

                foreach (string workerName in selectedWorkers)
                {
                    var id = getId(workerName);
                    var worker = workersService.getWorkerById(id);
                    
                    checkedWorkers.Add(worker);
                }
      
                DataTable data = new DataTable();

                data.Columns.Add("Работник");
                data.Columns.Add("Дата");
                data.Columns.Add("Сумма без отчёта");

                var reports = advanceReportsService.getAdvanceReportsList();

                var auditReport = reports
                    .Where(report =>
                        report.Date
                            .Contains(period.ToString()) &&
                        checkedWorkers
                            .Select(w => w.Id)
                            .Contains(report.AccountablePerson.Id)
                    )
                    .GroupBy(report => report.AccountablePerson)
                    .Select(group => group
                        .Select(g => new List<string> {
                            getWorkerInitialsWithId(group.Key),
                            g.Date,
                            g.AccountingRecords.Aggregate(
                                0.0, 
                                (sum, record) => sum + (record.Credit - record.Debit)
                            ).ToString(),
                        }).ToArray()
                    ).ToList();

                foreach (var group in auditReport)
                {
                    foreach (var row in group)
                    {
                        data.Rows.Add(row[0], row[1], row[2]);
                    }
                }

                tableView.DataSource = data;
                dialogSaveFile(data);
            }
        }

        private void dialogSaveFile(DataTable data)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog.FileName;

            auditService.exportReport(data, filename);

            MessageBox.Show("Файл сохранен");
        }

        private void auditAllWorkersPeriodInput_SelectedIndexChanged(object sender, EventArgs e){}
    }
}
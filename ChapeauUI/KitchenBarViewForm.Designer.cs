
namespace ChapeauG5.ChapeauUI.Forms
{
    partial class KitchenBarViewForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            KitchenBarFormHeaderPanel = new Panel();
            KitchenBarOrdersTitle = new Label();
            btnRefresh = new Button();
            btnCompletedOrders = new Button();
            btnPreparingOrders = new Button();
            btnRunningOrders = new Button();
            panelFiltersRunningOrderItems = new Panel();
            btnFilterDeserts = new Button();
            btnFilterMains = new Button();
            btnFilterStarters = new Button();
            btnFilterAll = new Button();
            panelSelectTableRunningOrderHeader = new Panel();
            label3 = new Label();
            panelSelectedTableRunningOrderHeader = new Panel();
            btnStartAll = new Button();
            btnReadyAll = new Button();
            SelectedRunningTableOrderNumber = new Label();
            flowPanelSelectTabelOrdersRunning = new FlowLayoutPanel();
            flowPanelSelectedTabelOrderItemsRunning = new FlowLayoutPanel();
            RunningOrdersPanel = new Panel();
            CompletedOrdersPanel = new Panel();
            flowPanelSelectedTabelOrderItemsCompleted = new FlowLayoutPanel();
            flowPanelSelectTabelOrdersCompleted = new FlowLayoutPanel();
            panelFiltersCompletedOrderItems = new Panel();
            btnDessertsFilter = new Button();
            btnMainsFilter = new Button();
            btnStartersFilter = new Button();
            btnAllFilter = new Button();
            panelSelectedTableCompletedOrderHeader = new Panel();
            SelectedCompletedTableOrderNumber = new Label();
            panelSelectTableCompletedOrderHeader = new Panel();
            labelCompletedTableOrders = new Label();
            flowPanelPreparingOrders = new FlowLayoutPanel();
            panelPreparingOrdersHeader = new Panel();
            TitlePreparingOrdersLabel = new Label();
            StatusComboBox = new ComboBox();
            SortBycomboBox = new ComboBox();
            StatusLabel = new Label();
            SortByLabel = new Label();
            preparingOrdersDataGridView = new DataGridView();
            OrderTableNumber = new DataGridViewTextBoxColumn();
            OrderItemTableNumber = new DataGridViewTextBoxColumn();
            TableNumber = new DataGridViewTextBoxColumn();
            OrderItemName = new DataGridViewTextBoxColumn();
            OrderItemQuantity = new DataGridViewTextBoxColumn();
            OrderItemStatus = new DataGridViewTextBoxColumn();
            OrderItemComment = new DataGridViewTextBoxColumn();
            OrderItemTime = new DataGridViewTextBoxColumn();
            OrderItemTimeWaited = new DataGridViewTextBoxColumn();
            OrderItemActions = new DataGridViewTextBoxColumn();
            PreparingOrdersPanel = new Panel();
            KitchenBarFormHeaderPanel.SuspendLayout();
            panelFiltersRunningOrderItems.SuspendLayout();
            panelSelectTableRunningOrderHeader.SuspendLayout();
            panelSelectedTableRunningOrderHeader.SuspendLayout();
            RunningOrdersPanel.SuspendLayout();
            CompletedOrdersPanel.SuspendLayout();
            panelFiltersCompletedOrderItems.SuspendLayout();
            panelSelectedTableCompletedOrderHeader.SuspendLayout();
            panelSelectTableCompletedOrderHeader.SuspendLayout();
            flowPanelPreparingOrders.SuspendLayout();
            panelPreparingOrdersHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)preparingOrdersDataGridView).BeginInit();
            PreparingOrdersPanel.SuspendLayout();
            SuspendLayout();
            // 
            // KitchenBarFormHeaderPanel
            // 
            KitchenBarFormHeaderPanel.BackColor = SystemColors.HotTrack;
            KitchenBarFormHeaderPanel.Controls.Add(KitchenBarOrdersTitle);
            KitchenBarFormHeaderPanel.Controls.Add(btnRefresh);
            KitchenBarFormHeaderPanel.Controls.Add(btnCompletedOrders);
            KitchenBarFormHeaderPanel.Controls.Add(btnPreparingOrders);
            KitchenBarFormHeaderPanel.Controls.Add(btnRunningOrders);
            KitchenBarFormHeaderPanel.Dock = DockStyle.Top;
            KitchenBarFormHeaderPanel.Location = new Point(0, 0);
            KitchenBarFormHeaderPanel.Name = "KitchenBarFormHeaderPanel";
            KitchenBarFormHeaderPanel.Size = new Size(1584, 109);
            KitchenBarFormHeaderPanel.TabIndex = 0;
            // 
            // KitchenBarOrdersTitle
            // 
            KitchenBarOrdersTitle.AutoSize = true;
            KitchenBarOrdersTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            KitchenBarOrdersTitle.Location = new Point(28, 19);
            KitchenBarOrdersTitle.Name = "KitchenBarOrdersTitle";
            KitchenBarOrdersTitle.Size = new Size(293, 45);
            KitchenBarOrdersTitle.TabIndex = 1;
            KitchenBarOrdersTitle.Text = "KitchenBar Orders";
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.MediumTurquoise;
            btnRefresh.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnRefresh.Location = new Point(1421, 13);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(86, 69);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "🔃";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnCompletedOrders
            // 
            btnCompletedOrders.BackColor = Color.DarkGray;
            btnCompletedOrders.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCompletedOrders.ForeColor = Color.Black;
            btnCompletedOrders.Location = new Point(1138, 13);
            btnCompletedOrders.Name = "btnCompletedOrders";
            btnCompletedOrders.Size = new Size(233, 69);
            btnCompletedOrders.TabIndex = 0;
            btnCompletedOrders.Text = "✓ Completed Orders";
            btnCompletedOrders.UseVisualStyleBackColor = false;
            btnCompletedOrders.Click += btnCompletedOrders_Click;
            // 
            // btnPreparingOrders
            // 
            btnPreparingOrders.BackColor = Color.DarkGray;
            btnPreparingOrders.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPreparingOrders.ForeColor = Color.Black;
            btnPreparingOrders.Location = new Point(881, 13);
            btnPreparingOrders.Name = "btnPreparingOrders";
            btnPreparingOrders.Size = new Size(233, 69);
            btnPreparingOrders.TabIndex = 0;
            btnPreparingOrders.Text = "🔥 Preparing Orders";
            btnPreparingOrders.UseVisualStyleBackColor = false;
            btnPreparingOrders.Click += btnPreparingOrders_Click;
            // 
            // btnRunningOrders
            // 
            btnRunningOrders.BackColor = Color.DarkGray;
            btnRunningOrders.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRunningOrders.Location = new Point(624, 13);
            btnRunningOrders.Name = "btnRunningOrders";
            btnRunningOrders.Size = new Size(233, 69);
            btnRunningOrders.TabIndex = 0;
            btnRunningOrders.Text = "◴ Running Orders";
            btnRunningOrders.UseVisualStyleBackColor = false;
            btnRunningOrders.Click += btnRunningOrders_Click;
            // 
            // panelFiltersRunningOrderItems
            // 
            panelFiltersRunningOrderItems.Controls.Add(btnFilterDeserts);
            panelFiltersRunningOrderItems.Controls.Add(btnFilterMains);
            panelFiltersRunningOrderItems.Controls.Add(btnFilterStarters);
            panelFiltersRunningOrderItems.Controls.Add(btnFilterAll);
            panelFiltersRunningOrderItems.Location = new Point(604, 93);
            panelFiltersRunningOrderItems.Name = "panelFiltersRunningOrderItems";
            panelFiltersRunningOrderItems.Size = new Size(892, 58);
            panelFiltersRunningOrderItems.TabIndex = 2;
            // 
            // btnFilterDeserts
            // 
            btnFilterDeserts.Location = new Point(569, 5);
            btnFilterDeserts.Name = "btnFilterDeserts";
            btnFilterDeserts.Size = new Size(98, 37);
            btnFilterDeserts.TabIndex = 0;
            btnFilterDeserts.Text = "Desserts";
            btnFilterDeserts.UseVisualStyleBackColor = true;
            btnFilterDeserts.Click += btnFilterDesserts_Click;
            // 
            // btnFilterMains
            // 
            btnFilterMains.Location = new Point(465, 4);
            btnFilterMains.Name = "btnFilterMains";
            btnFilterMains.Size = new Size(98, 37);
            btnFilterMains.TabIndex = 0;
            btnFilterMains.Text = "Mains";
            btnFilterMains.UseVisualStyleBackColor = true;
            btnFilterMains.Click += btnFilterMains_Click;
            // 
            // btnFilterStarters
            // 
            btnFilterStarters.Location = new Point(361, 4);
            btnFilterStarters.Name = "btnFilterStarters";
            btnFilterStarters.Size = new Size(98, 37);
            btnFilterStarters.TabIndex = 0;
            btnFilterStarters.Text = "Starters";
            btnFilterStarters.UseVisualStyleBackColor = true;
            btnFilterStarters.Click += btnFilterStarters_Click;
            // 
            // btnFilterAll
            // 
            btnFilterAll.Location = new Point(257, 4);
            btnFilterAll.Name = "btnFilterAll";
            btnFilterAll.Size = new Size(98, 37);
            btnFilterAll.TabIndex = 0;
            btnFilterAll.Text = "All";
            btnFilterAll.UseVisualStyleBackColor = true;
            btnFilterAll.Click += btnFilterAll_Click;
            // 
            // panelSelectTableRunningOrderHeader
            // 
            panelSelectTableRunningOrderHeader.BackColor = SystemColors.ActiveCaption;
            panelSelectTableRunningOrderHeader.Controls.Add(label3);
            panelSelectTableRunningOrderHeader.Location = new Point(50, 15);
            panelSelectTableRunningOrderHeader.Name = "panelSelectTableRunningOrderHeader";
            panelSelectTableRunningOrderHeader.Size = new Size(448, 77);
            panelSelectTableRunningOrderHeader.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(17, 23);
            label3.Name = "label3";
            label3.Size = new Size(182, 25);
            label3.TabIndex = 0;
            label3.Text = "Running Table Orders";
            // 
            // panelSelectedTableRunningOrderHeader
            // 
            panelSelectedTableRunningOrderHeader.BackColor = SystemColors.ActiveCaption;
            panelSelectedTableRunningOrderHeader.Controls.Add(btnStartAll);
            panelSelectedTableRunningOrderHeader.Controls.Add(btnReadyAll);
            panelSelectedTableRunningOrderHeader.Controls.Add(SelectedRunningTableOrderNumber);
            panelSelectedTableRunningOrderHeader.Location = new Point(604, 15);
            panelSelectedTableRunningOrderHeader.Name = "panelSelectedTableRunningOrderHeader";
            panelSelectedTableRunningOrderHeader.Size = new Size(892, 77);
            panelSelectedTableRunningOrderHeader.TabIndex = 2;
            // 
            // btnStartAll
            // 
            btnStartAll.Location = new Point(668, 23);
            btnStartAll.Name = "btnStartAll";
            btnStartAll.Size = new Size(98, 37);
            btnStartAll.TabIndex = 0;
            btnStartAll.Text = "Start All";
            btnStartAll.UseVisualStyleBackColor = true;
            btnStartAll.Click += btnStartAll_Click;
            // 
            // btnReadyAll
            // 
            btnReadyAll.Location = new Point(772, 23);
            btnReadyAll.Name = "btnReadyAll";
            btnReadyAll.Size = new Size(98, 37);
            btnReadyAll.TabIndex = 0;
            btnReadyAll.Text = "Ready All";
            btnReadyAll.UseVisualStyleBackColor = true;
            btnReadyAll.Click += btnReadyAll_Click;
            // 
            // SelectedRunningTableOrderNumber
            // 
            SelectedRunningTableOrderNumber.AutoSize = true;
            SelectedRunningTableOrderNumber.Location = new Point(25, 12);
            SelectedRunningTableOrderNumber.Name = "SelectedRunningTableOrderNumber";
            SelectedRunningTableOrderNumber.Size = new Size(68, 25);
            SelectedRunningTableOrderNumber.TabIndex = 0;
            SelectedRunningTableOrderNumber.Text = "Table #";
            // 
            // flowPanelSelectTabelOrdersRunning
            // 
            flowPanelSelectTabelOrdersRunning.BackColor = SystemColors.Control;
            flowPanelSelectTabelOrdersRunning.BorderStyle = BorderStyle.FixedSingle;
            flowPanelSelectTabelOrdersRunning.Location = new Point(50, 93);
            flowPanelSelectTabelOrdersRunning.Name = "flowPanelSelectTabelOrdersRunning";
            flowPanelSelectTabelOrdersRunning.Size = new Size(448, 582);
            flowPanelSelectTabelOrdersRunning.TabIndex = 3;
            // 
            // flowPanelSelectedTabelOrderItemsRunning
            // 
            flowPanelSelectedTabelOrderItemsRunning.BorderStyle = BorderStyle.Fixed3D;
            flowPanelSelectedTabelOrderItemsRunning.Location = new Point(604, 140);
            flowPanelSelectedTabelOrderItemsRunning.Name = "flowPanelSelectedTabelOrderItemsRunning";
            flowPanelSelectedTabelOrderItemsRunning.Size = new Size(892, 535);
            flowPanelSelectedTabelOrderItemsRunning.TabIndex = 3;
            // 
            // RunningOrdersPanel
            // 
            RunningOrdersPanel.Controls.Add(flowPanelSelectedTabelOrderItemsRunning);
            RunningOrdersPanel.Controls.Add(flowPanelSelectTabelOrdersRunning);
            RunningOrdersPanel.Controls.Add(panelFiltersRunningOrderItems);
            RunningOrdersPanel.Controls.Add(panelSelectedTableRunningOrderHeader);
            RunningOrdersPanel.Controls.Add(panelSelectTableRunningOrderHeader);
            RunningOrdersPanel.Location = new Point(30, 117);
            RunningOrdersPanel.Name = "RunningOrdersPanel";
            RunningOrdersPanel.Size = new Size(1526, 676);
            RunningOrdersPanel.TabIndex = 4;
            // 
            // CompletedOrdersPanel
            // 
            CompletedOrdersPanel.Controls.Add(flowPanelSelectedTabelOrderItemsCompleted);
            CompletedOrdersPanel.Controls.Add(flowPanelSelectTabelOrdersCompleted);
            CompletedOrdersPanel.Controls.Add(panelFiltersCompletedOrderItems);
            CompletedOrdersPanel.Controls.Add(panelSelectedTableCompletedOrderHeader);
            CompletedOrdersPanel.Controls.Add(panelSelectTableCompletedOrderHeader);
            CompletedOrdersPanel.Location = new Point(12, 124);
            CompletedOrdersPanel.Name = "CompletedOrdersPanel";
            CompletedOrdersPanel.Size = new Size(1526, 676);
            CompletedOrdersPanel.TabIndex = 4;
            CompletedOrdersPanel.Paint += CompletedOrdersPanel_Paint;
            // 
            // flowPanelSelectedTabelOrderItemsCompleted
            // 
            flowPanelSelectedTabelOrderItemsCompleted.BorderStyle = BorderStyle.FixedSingle;
            flowPanelSelectedTabelOrderItemsCompleted.Location = new Point(604, 140);
            flowPanelSelectedTabelOrderItemsCompleted.Name = "flowPanelSelectedTabelOrderItemsCompleted";
            flowPanelSelectedTabelOrderItemsCompleted.Size = new Size(892, 535);
            flowPanelSelectedTabelOrderItemsCompleted.TabIndex = 3;
            // 
            // flowPanelSelectTabelOrdersCompleted
            // 
            flowPanelSelectTabelOrdersCompleted.BackColor = SystemColors.Control;
            flowPanelSelectTabelOrdersCompleted.BorderStyle = BorderStyle.FixedSingle;
            flowPanelSelectTabelOrdersCompleted.Location = new Point(50, 93);
            flowPanelSelectTabelOrdersCompleted.Name = "flowPanelSelectTabelOrdersCompleted";
            flowPanelSelectTabelOrdersCompleted.Size = new Size(448, 582);
            flowPanelSelectTabelOrdersCompleted.TabIndex = 3;
            flowPanelSelectTabelOrdersCompleted.Paint += flowPanelSelectTabelOrdersCompleted_Paint;
            // 
            // panelFiltersCompletedOrderItems
            // 
            panelFiltersCompletedOrderItems.Controls.Add(btnDessertsFilter);
            panelFiltersCompletedOrderItems.Controls.Add(btnMainsFilter);
            panelFiltersCompletedOrderItems.Controls.Add(btnStartersFilter);
            panelFiltersCompletedOrderItems.Controls.Add(btnAllFilter);
            panelFiltersCompletedOrderItems.Location = new Point(604, 93);
            panelFiltersCompletedOrderItems.Name = "panelFiltersCompletedOrderItems";
            panelFiltersCompletedOrderItems.Size = new Size(892, 58);
            panelFiltersCompletedOrderItems.TabIndex = 2;
            // 
            // btnDessertsFilter
            // 
            btnDessertsFilter.Location = new Point(569, 3);
            btnDessertsFilter.Name = "btnDessertsFilter";
            btnDessertsFilter.Size = new Size(98, 37);
            btnDessertsFilter.TabIndex = 1;
            btnDessertsFilter.Text = "Desserts";
            btnDessertsFilter.UseVisualStyleBackColor = true;
            btnDessertsFilter.Click += btnDessertsFilter_Click;
            // 
            // btnMainsFilter
            // 
            btnMainsFilter.Location = new Point(465, 4);
            btnMainsFilter.Name = "btnMainsFilter";
            btnMainsFilter.Size = new Size(98, 37);
            btnMainsFilter.TabIndex = 0;
            btnMainsFilter.Text = "Mains";
            btnMainsFilter.UseVisualStyleBackColor = true;
            btnMainsFilter.Click += btnMainsFilter_Click;
            // 
            // btnStartersFilter
            // 
            btnStartersFilter.Location = new Point(361, 4);
            btnStartersFilter.Name = "btnStartersFilter";
            btnStartersFilter.Size = new Size(98, 37);
            btnStartersFilter.TabIndex = 0;
            btnStartersFilter.Text = "Starters";
            btnStartersFilter.UseVisualStyleBackColor = true;
            btnStartersFilter.Click += btnStartersFilter_Click;
            // 
            // btnAllFilter
            // 
            btnAllFilter.Location = new Point(257, 4);
            btnAllFilter.Name = "btnAllFilter";
            btnAllFilter.Size = new Size(98, 37);
            btnAllFilter.TabIndex = 0;
            btnAllFilter.Text = "All";
            btnAllFilter.UseVisualStyleBackColor = true;
            btnAllFilter.Click += btnAllFilter_Click;
            // 
            // panelSelectedTableCompletedOrderHeader
            // 
            panelSelectedTableCompletedOrderHeader.BackColor = Color.DarkSeaGreen;
            panelSelectedTableCompletedOrderHeader.Controls.Add(SelectedCompletedTableOrderNumber);
            panelSelectedTableCompletedOrderHeader.Location = new Point(604, 15);
            panelSelectedTableCompletedOrderHeader.Name = "panelSelectedTableCompletedOrderHeader";
            panelSelectedTableCompletedOrderHeader.Size = new Size(892, 77);
            panelSelectedTableCompletedOrderHeader.TabIndex = 2;
            // 
            // SelectedCompletedTableOrderNumber
            // 
            SelectedCompletedTableOrderNumber.AutoSize = true;
            SelectedCompletedTableOrderNumber.Location = new Point(25, 12);
            SelectedCompletedTableOrderNumber.Name = "SelectedCompletedTableOrderNumber";
            SelectedCompletedTableOrderNumber.Size = new Size(68, 25);
            SelectedCompletedTableOrderNumber.TabIndex = 0;
            SelectedCompletedTableOrderNumber.Text = "Table #";
            // 
            // panelSelectTableCompletedOrderHeader
            // 
            panelSelectTableCompletedOrderHeader.BackColor = Color.DarkSeaGreen;
            panelSelectTableCompletedOrderHeader.Controls.Add(labelCompletedTableOrders);
            panelSelectTableCompletedOrderHeader.Location = new Point(50, 15);
            panelSelectTableCompletedOrderHeader.Name = "panelSelectTableCompletedOrderHeader";
            panelSelectTableCompletedOrderHeader.Size = new Size(448, 77);
            panelSelectTableCompletedOrderHeader.TabIndex = 2;
            // 
            // labelCompletedTableOrders
            // 
            labelCompletedTableOrders.AutoSize = true;
            labelCompletedTableOrders.Location = new Point(17, 23);
            labelCompletedTableOrders.Name = "labelCompletedTableOrders";
            labelCompletedTableOrders.Size = new Size(204, 25);
            labelCompletedTableOrders.TabIndex = 0;
            labelCompletedTableOrders.Text = "Completed Table Orders";
            // 
            // flowPanelPreparingOrders
            // 
            flowPanelPreparingOrders.Controls.Add(panelPreparingOrdersHeader);
            flowPanelPreparingOrders.Controls.Add(preparingOrdersDataGridView);
            flowPanelPreparingOrders.Location = new Point(18, 8);
            flowPanelPreparingOrders.Name = "flowPanelPreparingOrders";
            flowPanelPreparingOrders.Size = new Size(1450, 678);
            flowPanelPreparingOrders.TabIndex = 4;
            // 
            // panelPreparingOrdersHeader
            // 
            panelPreparingOrdersHeader.Controls.Add(TitlePreparingOrdersLabel);
            panelPreparingOrdersHeader.Controls.Add(StatusComboBox);
            panelPreparingOrdersHeader.Controls.Add(SortBycomboBox);
            panelPreparingOrdersHeader.Controls.Add(StatusLabel);
            panelPreparingOrdersHeader.Controls.Add(SortByLabel);
            panelPreparingOrdersHeader.Location = new Point(3, 3);
            panelPreparingOrdersHeader.Name = "panelPreparingOrdersHeader";
            panelPreparingOrdersHeader.Size = new Size(1447, 78);
            panelPreparingOrdersHeader.TabIndex = 1;
            // 
            // TitlePreparingOrdersLabel
            // 
            TitlePreparingOrdersLabel.AutoSize = true;
            TitlePreparingOrdersLabel.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            TitlePreparingOrdersLabel.Location = new Point(55, 30);
            TitlePreparingOrdersLabel.Name = "TitlePreparingOrdersLabel";
            TitlePreparingOrdersLabel.Size = new Size(200, 32);
            TitlePreparingOrdersLabel.TabIndex = 2;
            TitlePreparingOrdersLabel.Text = "Preparing Orders";
            // 
            // StatusComboBox
            // 
            StatusComboBox.FormattingEnabled = true;
            StatusComboBox.Location = new Point(908, 27);
            StatusComboBox.Name = "StatusComboBox";
            StatusComboBox.Size = new Size(241, 33);
            StatusComboBox.TabIndex = 1;
            StatusComboBox.SelectedIndexChanged += StatusComboBox_SelectedIndexChanged;
            // 
            // SortBycomboBox
            // 
            SortBycomboBox.FormattingEnabled = true;
            SortBycomboBox.Location = new Point(513, 27);
            SortBycomboBox.Name = "SortBycomboBox";
            SortBycomboBox.Size = new Size(295, 33);
            SortBycomboBox.TabIndex = 1;
            SortBycomboBox.SelectedIndexChanged += SortBycomboBox_SelectedIndexChanged;
            // 
            // StatusLabel
            // 
            StatusLabel.AutoSize = true;
            StatusLabel.Location = new Point(832, 30);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(64, 25);
            StatusLabel.TabIndex = 0;
            StatusLabel.Text = "Status:";
            // 
            // SortByLabel
            // 
            SortByLabel.AutoSize = true;
            SortByLabel.Location = new Point(423, 30);
            SortByLabel.Name = "SortByLabel";
            SortByLabel.Size = new Size(74, 25);
            SortByLabel.TabIndex = 0;
            SortByLabel.Text = "Sort by:";
            // 
            // preparingOrdersDataGridView
            // 
            preparingOrdersDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            preparingOrdersDataGridView.Columns.AddRange(new DataGridViewColumn[] { OrderTableNumber, OrderItemTableNumber, TableNumber, OrderItemName, OrderItemQuantity, OrderItemStatus, OrderItemComment, OrderItemTime, OrderItemTimeWaited, OrderItemActions });
            preparingOrdersDataGridView.Location = new Point(3, 87);
            preparingOrdersDataGridView.Name = "preparingOrdersDataGridView";
            preparingOrdersDataGridView.RowHeadersWidth = 62;
            preparingOrdersDataGridView.Size = new Size(1447, 559);
            preparingOrdersDataGridView.TabIndex = 3;
            preparingOrdersDataGridView.CellContentClick += preparingOrdersDataGridView_CellContentClick;
            preparingOrdersDataGridView.Click += preparingOrdersDataGridView_Click;
            // 
            // OrderTableNumber
            // 
            OrderTableNumber.HeaderText = "Order#";
            OrderTableNumber.MinimumWidth = 8;
            OrderTableNumber.Name = "OrderTableNumber";
            OrderTableNumber.Resizable = DataGridViewTriState.False;
            OrderTableNumber.Width = 70;
            // 
            // OrderItemTableNumber
            // 
            OrderItemTableNumber.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            OrderItemTableNumber.HeaderText = "Order Item Number";
            OrderItemTableNumber.MinimumWidth = 8;
            OrderItemTableNumber.Name = "OrderItemTableNumber";
            OrderItemTableNumber.Width = 70;
            // 
            // TableNumber
            // 
            TableNumber.HeaderText = "Table #";
            TableNumber.MinimumWidth = 8;
            TableNumber.Name = "TableNumber";
            TableNumber.Width = 70;
            // 
            // OrderItemName
            // 
            OrderItemName.HeaderText = "ItemName";
            OrderItemName.MinimumWidth = 120;
            OrderItemName.Name = "OrderItemName";
            OrderItemName.Width = 150;
            // 
            // OrderItemQuantity
            // 
            OrderItemQuantity.HeaderText = "Quantity";
            OrderItemQuantity.MinimumWidth = 30;
            OrderItemQuantity.Name = "OrderItemQuantity";
            OrderItemQuantity.Width = 85;
            // 
            // OrderItemStatus
            // 
            OrderItemStatus.HeaderText = "Status";
            OrderItemStatus.MinimumWidth = 50;
            OrderItemStatus.Name = "OrderItemStatus";
            OrderItemStatus.Width = 150;
            // 
            // OrderItemComment
            // 
            OrderItemComment.HeaderText = "Comment";
            OrderItemComment.MinimumWidth = 20;
            OrderItemComment.Name = "OrderItemComment";
            OrderItemComment.Width = 150;
            // 
            // OrderItemTime
            // 
            OrderItemTime.HeaderText = "Order Time";
            OrderItemTime.MinimumWidth = 8;
            OrderItemTime.Name = "OrderItemTime";
            OrderItemTime.Width = 150;
            // 
            // OrderItemTimeWaited
            // 
            OrderItemTimeWaited.HeaderText = "Waiting";
            OrderItemTimeWaited.MinimumWidth = 8;
            OrderItemTimeWaited.Name = "OrderItemTimeWaited";
            OrderItemTimeWaited.Width = 150;
            // 
            // OrderItemActions
            // 
            OrderItemActions.HeaderText = "Actions";
            OrderItemActions.MinimumWidth = 8;
            OrderItemActions.Name = "OrderItemActions";
            OrderItemActions.Width = 150;
            // 
            // PreparingOrdersPanel
            // 
            PreparingOrdersPanel.Controls.Add(flowPanelPreparingOrders);
            PreparingOrdersPanel.Location = new Point(35, 119);
            PreparingOrdersPanel.Name = "PreparingOrdersPanel";
            PreparingOrdersPanel.Size = new Size(1497, 680);
            PreparingOrdersPanel.TabIndex = 5;
            // 
            // KitchenBarViewForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1584, 805);
            Controls.Add(KitchenBarFormHeaderPanel);
            Controls.Add(RunningOrdersPanel);
            Controls.Add(PreparingOrdersPanel);
            Controls.Add(CompletedOrdersPanel);
            Name = "KitchenBarViewForm";
            Text = "KitchenBarViewForm";
            KitchenBarFormHeaderPanel.ResumeLayout(false);
            KitchenBarFormHeaderPanel.PerformLayout();
            panelFiltersRunningOrderItems.ResumeLayout(false);
            panelSelectTableRunningOrderHeader.ResumeLayout(false);
            panelSelectTableRunningOrderHeader.PerformLayout();
            panelSelectedTableRunningOrderHeader.ResumeLayout(false);
            panelSelectedTableRunningOrderHeader.PerformLayout();
            RunningOrdersPanel.ResumeLayout(false);
            CompletedOrdersPanel.ResumeLayout(false);
            panelFiltersCompletedOrderItems.ResumeLayout(false);
            panelSelectedTableCompletedOrderHeader.ResumeLayout(false);
            panelSelectedTableCompletedOrderHeader.PerformLayout();
            panelSelectTableCompletedOrderHeader.ResumeLayout(false);
            panelSelectTableCompletedOrderHeader.PerformLayout();
            flowPanelPreparingOrders.ResumeLayout(false);
            panelPreparingOrdersHeader.ResumeLayout(false);
            panelPreparingOrdersHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)preparingOrdersDataGridView).EndInit();
            PreparingOrdersPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void flowPanelSelectTabelOrdersCompleted_Paint(object sender, PaintEventArgs e)
        {

        }

        #endregion

        private Panel KitchenBarFormHeaderPanel;
        private Button btnCompletedOrders;
        private Button btnPreparingOrders;
        private Button btnRunningOrders;
        private Button btnRefresh;
        private Label KitchenBarOrdersTitle;
        private Panel panelFiltersRunningOrderItems;
        private Button btnFilterMains;
        private Button btnFilterStarters;
        private Button btnFilterAll;
        private Panel panelSelectTableRunningOrderHeader;
        private Label label3;
        private Panel panelSelectedTableRunningOrderHeader;
        private Button btnReadyAll;
        private Label SelectedRunningTableOrderNumber;
        private Button btnStartAll;
        private FlowLayoutPanel flowPanelSelectTabelOrdersRunning;
        private KitchenBarOrderTables kitchenOrderTables1;
        private FlowLayoutPanel flowPanelSelectedTabelOrderItemsRunning;
        private KitchenBarOrderItems kitchenBarOrderItems1;
        private Panel RunningOrdersPanel;
        private Panel CompletedOrdersPanel;
        private FlowLayoutPanel flowPanelSelectedTabelOrderItemsCompleted;
        private KitchenBarOrderItems kitchenBarOrderItems3;
        private FlowLayoutPanel flowPanelSelectTabelOrdersCompleted;
        private KitchenBarOrderTables kitchenOrderTables3;
        private Panel panelFiltersCompletedOrderItems;
        private Button btnMainsFilter;
        private Button btnStartersFilter;
        private Button btnAllFilter;
        private Panel panelSelectedTableCompletedOrderHeader;
        private Label SelectedCompletedTableOrderNumber;
        private Panel panelSelectTableCompletedOrderHeader;
        private Label labelCompletedTableOrders;
        private FlowLayoutPanel flowPanelPreparingOrders;
        private Panel panelPreparingOrdersHeader;
        private Label TitlePreparingOrdersLabel;
        private ComboBox StatusComboBox;
        private ComboBox SortBycomboBox;
        private Label StatusLabel;
        private Label SortByLabel;
        private DataGridView preparingOrdersDataGridView;
        private Panel PreparingOrdersPanel;
        private Button btnFilterDeserts;
        private Button btnDessertsFilter;
        private DataGridViewTextBoxColumn OrderTableNumber;
        private DataGridViewTextBoxColumn OrderItemTableNumber;
        private DataGridViewTextBoxColumn TableNumber;
        private DataGridViewTextBoxColumn OrderItemName;
        private DataGridViewTextBoxColumn OrderItemQuantity;
        private DataGridViewTextBoxColumn OrderItemStatus;
        private DataGridViewTextBoxColumn OrderItemComment;
        private DataGridViewTextBoxColumn OrderItemTime;
        private DataGridViewTextBoxColumn OrderItemTimeWaited;
        private DataGridViewTextBoxColumn OrderItemActions;
    }
}
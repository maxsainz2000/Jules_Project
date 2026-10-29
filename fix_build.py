import re

with open("src/MerchSys.Inventory/Views/Dialogs/RecordShrinkageDialog.vb", "r") as f:
    content = f.read()

# Remove the duplicate OnLoad method block
content = content.replace("""        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            cmbProduct.Select()
        End Sub

""", "", 1)

with open("src/MerchSys.Inventory/Views/Dialogs/RecordShrinkageDialog.vb", "w") as f:
    f.write(content)

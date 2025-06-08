document.addEventListener('DOMContentLoaded', function() {
    // Dados de exemplo do inventário (Nome, Quantidade, Custo Unitário)
    // Em um sistema real, esses dados viriam de um backend/banco de dados
    const inventoryData = [
        { name: 'Arroz (saco 5kg)', quantity: 50, unitCost: 25.00 },
        { name: 'Feijão Carioca (kg)', quantity: 80, unitCost: 8.50 },
        { name: 'Óleo de Soja (900ml)', quantity: 100, unitCost: 7.00 },
        { name: 'Macarrão Espaguete (500g)', quantity: 150, unitCost: 4.50 },
        { name: 'Extrato de Tomate (340g)', quantity: 120, unitCost: 3.00 },
        { name: 'Farinha de Trigo (kg)', quantity: 60, unitCost: 5.50 },
        { name: 'Açúcar Refinado (kg)', quantity: 200, unitCost: 4.00 },
        { name: 'Sal Refinado (kg)', quantity: 70, unitCost: 2.00 },
        { name: 'Leite em Pó (400g)', quantity: 30, unitCost: 18.00 },
        { name: 'Café em Pó (500g)', quantity: 40, unitCost: 15.00 },
        { name: 'Bolacha Cream Cracker (pacote)', quantity: 250, unitCost: 2.80 },
        { name: 'Achocolatado em Pó (400g)', quantity: 35, unitCost: 9.00 },
        { name: 'Carne Bovina (kg - ex: acém)', quantity: 20, unitCost: 35.00 },
        { name: 'Frango (kg - coxa/sobrecoxa)', quantity: 45, unitCost: 12.00 },
        { name: 'Ovos (dúzia)', quantity: 60, unitCost: 9.00 },
        { name: 'Maçã (kg)', quantity: 30, unitCost: 7.50 },
        { name: 'Banana (kg)', quantity: 90, unitCost: 5.00 },
        { name: 'Batata (kg)', quantity: 100, unitCost: 4.20 },
        { name: 'Cebola (kg)', quantity: 50, unitCost: 3.80 },
        { name: 'Alho (cabeça)', quantity: 100, unitCost: 1.50 },
    ];

    // 1. Calcular valor total de cada item e o valor total do estoque
    let totalInventoryValue = 0;
    const itemsWithValue = inventoryData.map(item => {
        const totalValue = item.quantity * item.unitCost;
        totalInventoryValue += totalValue;
        return { ...item, totalValue };
    });

    // Atualizar resumo
    document.getElementById('total-items').textContent = inventoryData.length;
    document.getElementById('total-value').textContent = `R$ ${totalInventoryValue.toFixed(2).replace('.', ',')}`;

    // 2. Ordenar itens por valor total (decrescente)
    itemsWithValue.sort((a, b) => b.totalValue - a.totalValue);

    // 3. Calcular porcentagem acumulada do valor e classificar ABC
    let cumulativeValue = 0;
    const classifiedItems = itemsWithValue.map(item => {
        cumulativeValue += item.totalValue;
        const cumulativePercentage = (cumulativeValue / totalInventoryValue) * 100;
        let classification;

        if (cumulativePercentage <= 70) { // Categoria A: até 70% do valor acumulado
            classification = 'A';
        } else if (cumulativePercentage <= 95) { // Categoria B: de 70% a 95% do valor acumulado
            classification = 'B';
        } else { // Categoria C: acima de 95% do valor acumulado
            classification = 'C';
        }
        return { ...item, cumulativePercentage, classification, percentageOfTotalValue: (item.totalValue / totalInventoryValue) * 100 };
    });

    // 4. Popular as listas ABC e a tabela de inventário
    const listA = document.getElementById('list-a');
    const listB = document.getElementById('list-b');
    const listC = document.getElementById('list-c');
    const inventoryTableBody = document.getElementById('inventory-table-body');

    let countA = 0, countB = 0, countC = 0;

    classifiedItems.forEach(item => {
        // Adicionar à tabela de detalhes
        const row = inventoryTableBody.insertRow();
        row.insertCell().textContent = item.name;
        row.insertCell().textContent = item.quantity;
        row.insertCell().textContent = `R$ ${item.unitCost.toFixed(2).replace('.', ',')}`;
        row.insertCell().textContent = `R$ ${item.totalValue.toFixed(2).replace('.', ',')}`;
        row.insertCell().textContent = `${item.percentageOfTotalValue.toFixed(2)}%`;
        row.insertCell().textContent = item.classification;


        // Adicionar às listas ABC
        const listItem = document.createElement('li');
        listItem.innerHTML = `<span>${item.name}</span> (Valor: R$ ${item.totalValue.toFixed(2).replace('.', ',')})`;

        if (item.classification === 'A') {
            listA.appendChild(listItem);
            countA++;
        } else if (item.classification === 'B') {
            listB.appendChild(listItem);
            countB++;
        } else {
            listC.appendChild(listItem);
            countC++;
        }
    });

    // Atualizar contagem de itens por categoria
    document.querySelector('#category-a .item-count').textContent = `(${countA} itens)`;
    document.querySelector('#category-b .item-count').textContent = `(${countB} itens)`;
    document.querySelector('#category-c .item-count').textContent = `(${countC} itens)`;

});
# Day 26 — Charts Integration

## Objectives
- Chart.js integration with PrimeReact
- Multiple chart types
- Dynamic data updates

## Setup
```bash
npm install chart.js react-chartjs-2
```

## Chart Component
```tsx
import { Chart } from 'primereact/chart';

function ChartsDemo() {
  const [chartData, setChartData] = useState({});
  const [chartOptions, setChartOptions] = useState({});

  useEffect(() => {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');
    
    const data = {
      labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
      datasets: [
        {
          label: 'First Dataset',
          data: [65, 59, 80, 81, 56, 55, 40],
          fill: false,
          borderColor: documentStyle.getPropertyValue('--blue-500'),
          tension: 0.4
        },
        {
          label: 'Second Dataset',
          data: [28, 48, 40, 19, 86, 27, 90],
          fill: false,
          borderColor: documentStyle.getPropertyValue('--pink-500'),
          tension: 0.4
        }
      ]
    };
    
    const options = {
      maintainAspectRatio: false,
      aspectRatio: 0.6,
      plugins: {
        legend: {
          labels: {
            color: textColor
          }
        }
      },
      scales: {
        x: {
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder
          }
        },
        y: {
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder
          }
        }
      }
    };

    setChartData(data);
    setChartOptions(options);
  }, []);

  return (
    <div className="card">
      <Chart type="line" data={chartData} options={chartOptions} />
    </div>
  );
}
```

## Multiple Chart Types
```tsx
function ChartShowcase() {
  // Bar Chart Data
  const barData = {
    labels: ['Q1', 'Q2', 'Q3', 'Q4'],
    datasets: [
      {
        label: 'Sales',
        data: [540, 325, 702, 620],
        backgroundColor: ['rgba(255, 159, 64, 0.2)', 'rgba(75, 192, 192, 0.2)', 'rgba(54, 162, 235, 0.2)', 'rgba(153, 102, 255, 0.2)'],
        borderColor: ['rgb(255, 159, 64)', 'rgb(75, 192, 192)', 'rgb(54, 162, 235)', 'rgb(153, 102, 255)'],
        borderWidth: 1
      }
    ]
  };

  // Pie Chart Data
  const pieData = {
    labels: ['A', 'B', 'C'],
    datasets: [
      {
        data: [300, 50, 100],
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56'],
        hoverBackgroundColor: ['#FF6384', '#36A2EB', '#FFCE56']
      }
    ]
  };

  // Doughnut Chart Data
  const doughnutData = {
    labels: ['A', 'B', 'C'],
    datasets: [
      {
        data: [300, 50, 100],
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56'],
        hoverBackgroundColor: ['#FF6384', '#36A2EB', '#FFCE56']
      }
    ]
  };

  return (
    <div className="grid">
      <div className="col-12 md:col-6">
        <div className="card">
          <h5>Bar Chart</h5>
          <Chart type="bar" data={barData} />
        </div>
      </div>
      <div className="col-12 md:col-6">
        <div className="card">
          <h5>Pie Chart</h5>
          <Chart type="pie" data={pieData} />
        </div>
      </div>
      <div className="col-12 md:col-6">
        <div className="card">
          <h5>Doughnut Chart</h5>
          <Chart type="doughnut" data={doughnutData} />
        </div>
      </div>
    </div>
  );
}
```

## Exercise
- Create a dashboard with sales charts
- Add real-time data updates

## Checklist
- [ ] Line chart displays correctly
- [ ] Bar chart shows data
- [ ] Pie/Doughnut charts render
- [ ] Charts responsive to theme changes
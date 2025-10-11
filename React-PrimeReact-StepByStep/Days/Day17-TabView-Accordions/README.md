# Day 17 — TabView & Accordions

## Objectives
- Tabbed content organization
- Accordion layouts
- Dynamic tab management

## TabView
```tsx
function TabViewDemo() {
  const [activeIndex, setActiveIndex] = useState(0);

  return (
    <TabView activeIndex={activeIndex} onTabChange={(e) => setActiveIndex(e.index)}>
      <TabPanel header="Header I" leftIcon="pi pi-calendar">
        <p>Lorem ipsum dolor sit amet...</p>
      </TabPanel>
      <TabPanel header="Header II" rightIcon="pi pi-user">
        <p>Sed ut perspiciatis unde omnis...</p>
      </TabPanel>
      <TabPanel header="Header III" leftIcon="pi pi-search">
        <p>At vero eos et accusamus...</p>
      </TabPanel>
      <TabPanel header="Header IV" rightIcon="pi pi-cog">
        <p>Excepteur sint occaecat cupidatat...</p>
      </TabPanel>
    </TabView>
  );
}
```

## Dynamic Tabs
```tsx
function DynamicTabsDemo() {
  const [tabs, setTabs] = useState([
    { title: 'Tab 1', content: 'Content of Tab 1' },
    { title: 'Tab 2', content: 'Content of Tab 2' }
  ]);
  const [activeIndex, setActiveIndex] = useState(0);

  const addTab = () => {
    const newTab = {
      title: `Tab ${tabs.length + 1}`,
      content: `Content of Tab ${tabs.length + 1}`
    };
    setTabs([...tabs, newTab]);
  };

  const removeTab = (index) => {
    const newTabs = tabs.filter((_, i) => i !== index);
    setTabs(newTabs);
    if (activeIndex >= newTabs.length) {
      setActiveIndex(Math.max(0, newTabs.length - 1));
    }
  };

  return (
    <div>
      <Button label="Add Tab" onClick={addTab} className="p-mb-2" />
      <TabView activeIndex={activeIndex} onTabChange={(e) => setActiveIndex(e.index)}>
        {tabs.map((tab, index) => (
          <TabPanel 
            key={index} 
            header={
              <div className="p-d-flex p-ai-center">
                <span>{tab.title}</span>
                <Button 
                  icon="pi pi-times" 
                  className="p-button-text p-button-sm p-ml-2" 
                  onClick={(e) => {
                    e.stopPropagation();
                    removeTab(index);
                  }} 
                />
              </div>
            }
          >
            <p>{tab.content}</p>
          </TabPanel>
        ))}
      </TabView>
    </div>
  );
}
```

## Accordion
```tsx
function AccordionDemo() {
  const [activeIndex, setActiveIndex] = useState([0]);

  return (
    <Accordion 
      multiple 
      activeIndex={activeIndex} 
      onTabChange={(e) => setActiveIndex(e.index)}
    >
      <AccordionTab header="Header I">
        <p>Lorem ipsum dolor sit amet...</p>
      </AccordionTab>
      <AccordionTab header="Header II">
        <p>Sed ut perspiciatis unde omnis...</p>
      </AccordionTab>
      <AccordionTab header="Header III">
        <p>At vero eos et accusamus...</p>
      </AccordionTab>
    </Accordion>
  );
}
```

## Exercise
- Build a settings page with tabs for different categories
- Create an FAQ section using accordions

## Checklist
- [ ] Tabs switch correctly
- [ ] Dynamic tab addition/removal works
- [ ] Accordions expand/collapse
- [ ] Multiple accordion panels can be open
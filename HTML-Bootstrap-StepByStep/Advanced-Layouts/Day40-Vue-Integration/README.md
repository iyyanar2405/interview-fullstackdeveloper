# Day 40: Bootstrap + Vue Integration 🖖

Integrate Bootstrap with Vue.js using BootstrapVue.

```vue
<!-- npm install bootstrap-vue-next bootstrap -->
<template>
  <BContainer class="py-5">
    <BCard>
      <BCardBody>
        <BCardTitle>Vue Bootstrap</BCardTitle>
        <BCardText>Bootstrap components for Vue 3</BCardText>
        <BButton variant="primary">Click Me</BButton>
      </BCardBody>
    </BCard>
  </BContainer>
</template>

<script setup>
import { BContainer, BCard, BCardBody, BCardTitle, BCardText, BButton } from 'bootstrap-vue-next';
import 'bootstrap/dist/css/bootstrap.min.css';
</script>
```

**Next: Day 41 - Bootstrap + Angular Integration** 🅰️

<script setup lang="ts">
defineProps<{
  error : string
}>();
</script>

<template>
  <div v-if="error">
    <div class="alert alert-danger" role="alert">
      <p class="mt-1">{{ error }}</p>
    </div>
  </div>
</template>

<style scoped>

</style>
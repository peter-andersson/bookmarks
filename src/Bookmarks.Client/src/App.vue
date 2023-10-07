<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <nav class="nav justify-content-center">
    <RouterLink class="nav-link" active-class="active" to="/">Bookmark</RouterLink>
    <RouterLink class="nav-link" active-class="active" to="/add">Add bookmark</RouterLink>
  </nav>

  <div class="container">
    <RouterView />
  </div>
</template>
